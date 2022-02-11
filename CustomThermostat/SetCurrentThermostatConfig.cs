using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemon3Apps
{
    public class SetCurrentThermostatConfig
    {
        public SetCurrentThermostatConfig_Sensor[] TempSensors { get; set; }
    }

    public class SetCurrentThermostatConfig_Sensor
    {
        public string SensorId { get; set; }
        public string Alias { get; set; }
        public string FromTime { get; set; }
        
        public string ToTime { get; set; }
    }
}
