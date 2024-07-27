using System.Text.Json.Serialization;
using HardWareMonitorService.Entity.Monitoring;

namespace HardWareMonitorService.Utils;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PcMonitoringInfo))]
public partial class JsonContext : JsonSerializerContext
{
    
}
