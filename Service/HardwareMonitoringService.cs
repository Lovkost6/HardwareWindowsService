using System.Diagnostics;
using HardWareMonitorService.Entity.Monitoring;
using LibreHardwareMonitor.Hardware;

namespace HardWareMonitorService.Service;

public class HardwareMonitoringService
{
    private PcMonitoringInfo pcMonitoringInfo = new();

    public PcMonitoringInfo GetPcRTInfo()
    {
        var computer = new Computer();
        computer.Open();
        pcMonitoringInfo.GpuRt = GetGpuRT(computer);
        pcMonitoringInfo.CpuRt = GetCpuRT(computer);
        pcMonitoringInfo.RamRt = GetRamRT(computer);
        pcMonitoringInfo.NetworkRt = GetNetworkRT(computer);
        computer.Close();
        return pcMonitoringInfo;
    }

    private GpuRT GetGpuRT(Computer computer)
    {
        computer.IsGpuEnabled = true;
        computer.Hardware[0].Update();
        var gpuRt = new GpuRT();
        foreach (var sensor in computer.Hardware[0].Sensors)
        {
            if (sensor.Name == "D3D Video Codec 0")
            {
                gpuRt.TotalLoad = (int)sensor.Value.GetValueOrDefault();
            }

            if (sensor.Name == "GPU Core" && sensor.SensorType == SensorType.Temperature)
            {
                gpuRt.Temperature = (int)sensor.Value.GetValueOrDefault();
            }

            if (sensor.Name == "GPU Fan")
            {
                gpuRt.Fan = (int)sensor.Value.GetValueOrDefault();
            }
        }

        computer.IsGpuEnabled = false;
        return gpuRt;
    }

    private NetworkRT GetNetworkRT(Computer computer)
    {
        computer.IsNetworkEnabled = true;
        var networkRt = new NetworkRT();
        computer.Hardware[0].Update();
        foreach (var sensor in computer.Hardware[0].Sensors)
        {
            if (sensor.Name == "Upload Speed")
            {
                networkRt.Upload = (float)Math.Round(sensor.Value.GetValueOrDefault() / 1000 / 1000, 2);
            }

            if (sensor.Name == "Download Speed")
            {
                networkRt.Download = (float)Math.Round(sensor.Value.GetValueOrDefault() / 1000 / 1000, 2);
            }
        }

        computer.IsNetworkEnabled = false;
        return networkRt;
    }

    private CpuRT GetCpuRT(Computer computer)
    {
        computer.IsCpuEnabled = true;
        var cpuRT = new CpuRT();
        computer.Hardware[0].Update();
        foreach (var sensor in computer.Hardware[0].Sensors)
        {
            if (sensor.Name == "Core (SVI2 TFN)")
            {
                cpuRT.Voltage = (float)Math.Round(sensor.Value.GetValueOrDefault(), 2);
            }

            if (sensor.Name == "Core (Tctl/Tdie)")
            {
                cpuRT.Temperature = (float)Math.Round(sensor.Value.GetValueOrDefault(), 1);
            }
        }

        var cp = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        var use = (int)cp.NextValue();
        Thread.Sleep(1000);
        use = (int)cp.NextValue();


        cpuRT.TotalLoad = use;
        computer.IsCpuEnabled = false;
        return cpuRT;
    }

    private RamRT GetRamRT(Computer computer)
    {
        computer.IsMemoryEnabled = true;
        computer.Hardware[0].Update();
        var ramRT = new RamRT()
        {
            Used = (float)Math.Round(computer.Hardware[0].Sensors[0].Value.GetValueOrDefault(), 2),
            Available = (float)Math.Round(computer.Hardware[0].Sensors[1].Value.GetValueOrDefault(), 2),
            Load = (int)Math.Round(computer.Hardware[0].Sensors[2].Value.GetValueOrDefault(), 2),
        };
        computer.IsMemoryEnabled = false;
        return ramRT;
    }
}