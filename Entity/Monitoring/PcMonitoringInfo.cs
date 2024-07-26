namespace HardWareMonitorService.Entity.Monitoring;

public class PcMonitoringInfo
{
    public GpuRT GpuRt { get; set; }
    public CpuRT CpuRt { get; set; }
    public NetworkRT NetworkRt { get; set; }
    public RamRT RamRt { get; set; }
}