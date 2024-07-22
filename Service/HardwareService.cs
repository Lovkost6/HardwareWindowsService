using System.Management;
using System.Security.Cryptography;
using System.Text;
using HardWareMonitorService.Entity;
using LibreHardwareMonitor.Hardware;

namespace HardWareMonitorService.Service;

public class HardwareService
{
    public UserPc GetUserPc()
    {
        var userPc = new UserPc()
        {
            Cpu = GetCpuInfo(),
            Gpu = GetGpuInfo(),
            MotherBoard = GetMotherBoardInfo(),
            Os = GetOsInfo(),
            Rams = GetRamInfo(),
            Storages = GetStorageInfo()
        };
        return userPc;
    }

    public string GetUserPCId()
    {
        var macAddres = "";
        ManagementObjectSearcher os =
            new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

        foreach (ManagementObject queryObj in os.Get())
        {
            if (queryObj["Description"].ToString().Contains("Realtek"))
            {
                macAddres = queryObj["MACAddress"].ToString();
                break;
            }
        }

        var machineName = GetOsInfo().MachineName;
        var inputBytes = Encoding.UTF8.GetBytes(macAddres+machineName);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }

    private List<Storage> GetStorageInfo()
    {
        var listStorage = new List<Storage>(2);
        ManagementObjectSearcher queryOs = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
        foreach (var x in queryOs.Get())
        {
            var storage = new Storage()
            {
                Name = x["Caption"].ToString().Trim(),
                TotalSize = (int)((ulong) x["Size"]/1000/1000/1000),
            };
            listStorage.Add(storage);
        }

        return listStorage;
    }

    private List<Ram> GetRamInfo()
    {
        var listRam = new List<Ram>();
        ManagementObjectSearcher queryRam = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
        foreach (var x in queryRam.Get())
        {
            var ram = new Ram()
            {
                Capacity = (int) ((ulong)x["Capacity"]/1024/1024),
                Name = x["PartNumber"].ToString().Trim(),
                Speed = (int) ((uint)x["ConfiguredClockSpeed"])
            };
            listRam.Add(ram);
        }

        return listRam;
    }
    
    private MotherBoard GetMotherBoardInfo()
    {
        var computer = new Computer
        {
            IsMotherboardEnabled = true,
        };
        computer.Open();
        var motherboard = new MotherBoard()
        {
            Name = computer.Hardware[0].Name
        };
        computer.Close();
        return motherboard;
    }
     
    private Gpu GetGpuInfo()
    {
        ManagementObjectSearcher queryGpu = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
        var gpu = queryGpu.Get().GetEnumerator();
        gpu.MoveNext();
        var gpuObj = new Gpu()
        {
            Name = gpu.Current["Caption"].ToString().Trim(),
            AdapterRam = (int)((uint) gpu.Current["AdapterRAM"]/1024/1024),
            Driver = gpu.Current["DriverVersion"].ToString()
        };
        return gpuObj;
    }
    
    private Cpu GetCpuInfo()
    {
        ManagementObjectSearcher queryCpu = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        var cpu = queryCpu.Get().GetEnumerator();
        cpu.MoveNext();
        var cpuObj = new Cpu()
        {
            Threads = (int)((uint)cpu.Current["NumberOfLogicalProcessors"]),
            Socket = cpu.Current["SocketDesignation"].ToString().Trim(),
            Cores = (int)((uint)cpu.Current["NumberOfCores"]),
            Name = cpu.Current["Name"].ToString().Trim(),
            Freq = (int)((uint)cpu.Current["MaxClockSpeed"]),
        };
        return cpuObj;
    }
    private OperationSystem GetOsInfo()
    {
        ManagementObjectSearcher queryOs =
            new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
        var os = queryOs.Get().GetEnumerator();
        os.MoveNext();
        var operationSys = new OperationSystem()
        {
            Build = os.Current["Version"].ToString().Trim(),
            Caption = os.Current["Caption"].ToString().Trim(),
            MachineName = Environment.MachineName,
            OsArchitecture = os.Current["OSArchitecture"].ToString().Trim()
        };
        return operationSys;
    }
    
}