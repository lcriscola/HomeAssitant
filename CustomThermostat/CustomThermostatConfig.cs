using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemon3Apps
{
    public class CustomThermostatConfig
    {
        public string? ThermostatId { get; set; }
        public CustomThermostat_Sensor[] TempSensors { get; set; }
    }

    public class CustomThermostat_Sensor
    {
        public string SensorId { get; set; }
        public string Alias { get; set; }
    }
}
