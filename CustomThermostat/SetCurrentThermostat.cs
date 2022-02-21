using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using NetDaemon.AppModel;
using NetDaemon.HassModel;

using NetDaemon.Extensions.MqttEntityManager;
using NetDaemon.Extensions.MqttEntityManager.Models;
using NetDaemon.HassModel.Entities;

namespace NetDaemon3Apps
{


    [NetDaemonApp]
    public class SetCurrentThermostat : IAsyncInitializable
    {
        private readonly IHaContext _ha;
        private readonly ILogger<SetCurrentThermostat> _logger;
        private readonly IAppConfig<SetCurrentThermostatConfig> _config;
        private readonly IMqttEntityManager _entityManager;

        public SetCurrentThermostat(IHaContext ha, ILogger<SetCurrentThermostat> logger, IAppConfig<SetCurrentThermostatConfig> config, IMqttEntityManager entityManager)
        {
            _ha = ha;
            _logger = logger;
            _config = config;
            _entityManager = entityManager;
            _logger.Log(LogLevel.Information, $"{nameof(SetCurrentThermostat)} started.");


            foreach (var sensor in _config.Value.TempSensors)
            {
                _logger.Log(LogLevel.Information, $"Subscribing  to  {sensor.Alias} {sensor.SensorId}");

                ha.Entity(sensor.SensorId)
                    .StateChanges()
                    .Subscribe(async x =>
                    {
                        eventCounter++;
                        var temp = Convert.ToDecimal(x.New.State);
                        temperatures[sensor.SensorId]= temp;

                        await SetState(temp, sensor.Alias, sensor.SensorId);

                        _logger.Log(LogLevel.Information, $"#{eventCounter} {sensor.Alias} = {temp} .");
                    });

            }

          
        }
        int eventCounter = 0;
        Dictionary<string, decimal> temperatures = new Dictionary<string, decimal>();   

        void DumpObject(object obj)
        {
            if (obj==null)
                _logger.LogInformation($"#{eventCounter} -- NULL --");


            _logger.LogInformation($"#{eventCounter} -- {obj} {obj.GetType().Name} --");

            var props = obj.GetType().GetProperties();
            foreach (var p in props)
            {
                _logger.LogInformation($"#{eventCounter} {p.Name}");
            }
        }

        const string SensorId = "sensor.temperature_sensor_current";

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {

            await _entityManager.CreateAsync(SensorId, new EntityCreationOptions()
            {
                DeviceClass = DEVICE_CLASS_TEMPERATURE,
                Name="Temperature Sensor Current",
                Persist=true
            });


            return;
        }


        private async Task SetState(decimal temperature, string sensorAlias, string sensorId)
        {
            var dic = new Dictionary<string, object>();
            dic["measuredOn"] = DateTime.Now;
            dic["sensorAlias"] = sensorAlias;
            dic["sensorId"] = sensorId;
            await _entityManager.UpdateAsync(SensorId, temperature.ToString("N2"), dic);

        }
        const string DEVICE_CLASS_TEMPERATURE  = "temperature";
    }
}
