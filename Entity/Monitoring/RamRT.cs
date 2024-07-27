using System.Text.Json.Serialization;

namespace HardWareMonitorService.Entity.Monitoring;

public class RamRT
{
    public float Used { get; set; }
    public float Available { get; set; }
    public float Load { get; set; }
}