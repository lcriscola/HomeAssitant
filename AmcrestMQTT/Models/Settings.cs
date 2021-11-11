using System.Text.Json.Serialization;

namespace AmcrestMQTT.Models
{
    public class Settings
    {
        [JsonPropertyName("mqtt_host")]
        public string MQTT_Host { get; set; }
        public string MQTT_User { get; set; }
        public string MQTT_Password { get; set; }
        public CameraSetting[] Cameras { get; set; }
    }

    public class CameraSetting
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }

    }

}
