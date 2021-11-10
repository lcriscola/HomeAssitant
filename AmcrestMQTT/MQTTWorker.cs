using AmcrestMQTT.Models;
using AmcrestMQTT.Topics;

using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Mqtt;
using System.Text;

namespace AmcrestMQTT
{
    public class MQTTWorker : IHostedService, IDisposable
    {
        IMqttClient? client;
        private readonly IOptions<Settings> _options;

        public void Dispose()
        {
        }

        public MQTTWorker(IOptions<Settings> options)
        {
            this._options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Connect();
            await SendDiscoveryInformationAsync();


            ConnectToCameras();
        }

        private async Task Connect()
        {
            client = await MqttClient.CreateAsync(_options.Value.MQQT_Host);
            await client.ConnectAsync(new MqttClientCredentials("amcrest2mqtt", _options.Value.MQQT_UserName, _options.Value.MQQT_Password));
        }

        private void ConnectToCameras()
        {
            foreach (var cam in Camera.GetCameras())
            {
                StartCameraThread(cam);
            }
        }

        private   void StartCameraThread(Camera cam)
        {
            Thread t = new Thread(async (object state) =>
            {
                try
                {
                    if (client == null)
                    {
                        Connect();
                    }
                    Camera camera = (Camera)state;

                    var http = new HttpClient();

                    var sensors = Sensor.GetSensors();
                    var url = $"http://{camera.Host}/cgi-bin/eventManager.cgi?action=attach&codes=[{String.Join(",", sensors.Select(x => x.Code))}]";
                    var credCache = new CredentialCache();
                    credCache.Add(new Uri(url), "Digest", new NetworkCredential("admin", camera.Password));
                    var httpClient = new HttpClient(new HttpClientHandler { Credentials = credCache });
                    var response = await httpClient.GetStreamAsync(url);

                    StreamReader sr = new StreamReader(response);
                    var line = "";


                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "--myboundary")
                        {
                            // var st = File.OpenWrite(file);

                            var amcrest = await AmcrestStreamedResponse.Parse(sr);
                            var sensor = sensors.FirstOrDefault(x => x.Code == amcrest.Code);
                            if (sensor != null)
                            {
                                if (amcrest.Action.ToUpper() == "START")
                                    await SendStatus(sensor.GetStateTopic(camera.UniqueId), "ON");
                                else
                                    await SendStatus(sensor.GetStateTopic(camera.UniqueId), "OFF");
                            }


                            string text = $"{DateTime.Now} {amcrest.Code,-20} {amcrest.Action,-20} {amcrest.Index}";
                            Console.WriteLine(text);

                            var data = System.Text.Encoding.UTF8.GetBytes(text);
                            //await st.WriteAsync(System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString() + Environment.NewLine));
                            //await st.WriteAsync(data, 0, data.Length);
                            //await st.FlushAsync();
                            //await st.DisposeAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    client.Dispose();
                    client= null;
                    Console.Error.WriteLine(ex.Message);
                }

            });
            t.Start(cam);
        }

        private async Task SendDiscoveryInformationAsync()
        {
            var sensors = Topics.Sensor.GetSensors();
            foreach (var sensor in sensors)
            {

                foreach (var camera in Camera.GetCameras())
                {
                    var configTopic = sensor.GetConfigTopic(camera.UniqueId);
                    var data = new
                    {
                        name = camera.Name + " "+sensor.Name,
                        unique_id = camera.UniqueId+"_"+sensor.UniqueId,
                        device_class = sensor.DeviceClass,
                        state_topic = sensor.GetStateTopic(camera.UniqueId)
                    };
                    var text = System.Text.Json.JsonSerializer.Serialize(data);
                    await client.PublishAsync(new MqttApplicationMessage(configTopic, Encoding.UTF8.GetBytes(text)), MqttQualityOfService.AtLeastOnce);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await client.DisconnectAsync();
            client.Dispose();
        }


        async Task SendStatus(string topic, string status)
        {
            var text =   status ;
            await client.PublishAsync(new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(text)), MqttQualityOfService.AtLeastOnce);
            //Console.Write($"Message sent to {topic} with  {status}");
        }
    }
}
