namespace AmcrestMQTT.Topics
{
    public class Sensor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Component { get; set; }
        public string UniqueId { get;   set; }
        public string DeviceClass { get; set; }
        public string Code { get; set; }
        public string GetBaseTopic(string name)
        {
            return $"homeassistant/{this.Component}/{name}_{this.UniqueId}";
        }
        public string GetConfigTopic(string name)
        {
            return $"{this.GetBaseTopic(name)}/config";
        }
        public string GetStateTopic(string name)
        {
            return $"{this.GetBaseTopic(name)}/state";
        }



        static List<Sensor> _GetSensors = null;
        public static List<Sensor> GetSensors()
        {
            if (_GetSensors == null)
            {
                _GetSensors = new List<Sensor>()
            {
                new Sensor(){ Code="VideoMotion", DeviceClass="motion", Component="binary_sensor", Name="Motion Detected", UniqueId="motion_detected_sensor" },
                new Sensor(){ Code="CrossRegionDetection",DeviceClass="motion", Component="binary_sensor", Name=" Human Motion Detected", UniqueId="human_motion_detected_sensor" },
                new Sensor(){ Code="_CallRemoveMask",DeviceClass="power", Component="binary_sensor", Name=" Doorbell Pressed", UniqueId="doorbell_pressed" }
            };
            }

            return _GetSensors;
        }

    }


}
