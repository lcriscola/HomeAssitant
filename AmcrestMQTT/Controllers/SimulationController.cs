using AmcrestMQTT.Models;
using AmcrestMQTT.Topics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace AmcrestMQTT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimulationController : ControllerBase
    {
 
        private readonly ILogger<SimulationController> _logger;
 
        public SimulationController(ILogger<SimulationController> logger, Settings options)
        {
            _logger = logger;
            _options = options;
        }

        private readonly Settings _options;

        [HttpGet()]
        [Route("simulate_on")]
        public async Task<IActionResult> On(string cameraId, string eventName)
        {
            try
            {
                await SendSensorValue(cameraId, eventName, "ON");
                return this.Ok();
            }
            catch (ArgumentNullException ex)
            {
                return this.NotFound(ex.Message);
            }
        }
        [HttpGet()]
        [Route("simulate_off")]

        public async Task<IActionResult> Off(string cameraId, string eventName)
        {
            try
            {
                await SendSensorValue(cameraId, eventName, "OFF");
                return this.Ok();
            }
            catch (ArgumentNullException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        private async Task SendSensorValue(string cameraId, string eventName, string value)
        {
            var client = await MqttClient.CreateAsync(_options.MQTT_Host);
            await client.ConnectAsync(new MqttClientCredentials("amcrest2mqtt", _options.MQTT_User, _options.MQTT_Password));


            var cameras= Camera.GetCameras();
            var cam =cameras.FirstOrDefault(x => x.UniqueId == cameraId);
            if (cam==null)
                throw new ArgumentNullException($"Camera {cameraId} not found");

            var sensors = Sensor.GetSensors();
            var sensor = sensors.FirstOrDefault(x => x.Code == eventName);
            if (sensor == null)
                throw new ArgumentNullException($"Sensor hanlding event {eventName} not found");

            var topic = sensor.GetStateTopic(cameraId);
            Console.WriteLine($"{topic} {value}");
            await client.PublishAsync(new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(value)), MqttQualityOfService.AtLeastOnce);
        }
    }
}