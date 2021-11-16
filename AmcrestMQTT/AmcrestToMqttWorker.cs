using AmcrestMQTT.Models;
using AmcrestMQTT.Topics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mqtt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmcrestMQTT
{
    public class AmcrestToMqttWorker
    {
        private readonly Settings _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public AmcrestToMqttWorker(Settings options, IHttpClientFactory httpClientFactory)
        {
            this._options = options;
            this._httpClientFactory = httpClientFactory;
        }

        public void ConnectToCameras()
        {
            Console.WriteLine("Setting up cameras ...");
            foreach (var cam in _options.Cameras)
            {
                Console.WriteLine($"\t{cam.Name}");
                StartCameraThread(cam);
            }
        }

        void StartCameraThread(CameraSetting cam)
        {
            HttpClient httpClient;
            Thread keepAliveThread = new Thread(async (object state) =>
            {

            });
            Thread streamRequestThread = new Thread(async (object state) =>
            {
                while (true)
                {
                    try
                    {
                        CameraSetting camera = (CameraSetting)state;

                        using (var http = _httpClientFactory.CreateClient())
                        {


                            var sensors = _options.Sensors;
                            var events = String.Join(",", sensors.Select(x => x.Code));
                            if (_options.ListenToAllEvents)
                                events = "All";

                            var url = $"http://{camera.Host}/cgi-bin/eventManager.cgi?action=attach&codes=[{events}]";
                            var credCache = new CredentialCache();
                            credCache.Add(new Uri(url), "Digest", new NetworkCredential("admin", camera.Password));
                            var httpClient = new HttpClient(new HttpClientHandler { Credentials = credCache });
                            var stream = await httpClient.GetStreamAsync(url);
                            StreamReader sr = new StreamReader(stream);
                            var line = "";

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line == "--myboundary")
                                {
                                    var amcrest = await AmcrestStreamedResponse.Parse(sr);
                                    var sensor = sensors.FirstOrDefault(x => x.Code == amcrest.Code);
                                    if (sensor != null)
                                    {
                                        if (amcrest.Action.ToUpper() == "START")
                                            await SendStatus(sensor.GetStateTopic(camera.UniqueId), "ON");
                                        else
                                            await SendStatus(sensor.GetStateTopic(camera.UniqueId), "OFF");
                                    }
                                    else
                                    {
                                        Console.Write($" **** Unhandled Event ****");
                                    }

                                    string text = $"{DateTime.Now} {amcrest.Code,-20} {amcrest.Action,-20} {amcrest.Index}";
                                    Console.WriteLine(text);

                                    var data = System.Text.Encoding.UTF8.GetBytes(text);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.ToString());
                        Thread.Sleep(1000);
                    }
                }

            });
            streamRequestThread.Start(cam);
            keepAliveThread.Start();
        }

       public async Task SendStatus(string topic, string status)
        {
            IMqttClient mqttClient = await GetMqttClient();
            var text = status;
            await mqttClient.PublishAsync(new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(text)), MqttQualityOfService.AtLeastOnce);
            Console.WriteLine($"\tMessage sent to {topic} with  {status}");
            await mqttClient.DisconnectAsync();
            mqttClient.Dispose();
        }

        public async Task SendDiscoveryInformationAsync()
        {
            Console.WriteLine("Sending discovery information ...");
            IMqttClient mqttClient = await GetMqttClient();

            var sensors = _options.Sensors;
            foreach (var sensor in sensors)
            {

                foreach (var camera in _options.Cameras)
                {
                    var configTopic = sensor.GetConfigTopic(camera.UniqueId);
                    var data = new
                    {
                        name = camera.Name + " " + sensor.Name,
                        unique_id = camera.UniqueId + "_" + sensor.UniqueId,
                        device_class = sensor.DeviceClass,
                        state_topic = sensor.GetStateTopic(camera.UniqueId)
                    };
                    var text = System.Text.Json.JsonSerializer.Serialize(data);
                    await mqttClient.PublishAsync(new MqttApplicationMessage(configTopic, Encoding.UTF8.GetBytes(text)), MqttQualityOfService.AtLeastOnce);
                    Console.WriteLine($"\t{configTopic}");
                }
            }
        }

        private async Task<IMqttClient> GetMqttClient()
        {
            var mqttClient = await MqttClient.CreateAsync(_options.MQTT_Host);
            await mqttClient.ConnectAsync(new MqttClientCredentials("amcrest2mqtt", _options.MQTT_User, _options.MQTT_Password));
            return mqttClient;
        }

    
    }
}
