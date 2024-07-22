using System.ComponentModel.DataAnnotations.Schema;

namespace HardWareMonitorService.Entity;
[Table("Os")]
public class OperationSystem
{
    // public long Id { get; set; }
    public string Caption { get; set; }
    public string Build { get; set; }
    public string OsArchitecture { get; set; }
    public string MachineName { get; set; }
}