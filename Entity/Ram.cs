using System.ComponentModel.DataAnnotations.Schema;

namespace HardWareMonitorService.Entity;

[Table("Ram")]
public class Ram
{
    // public long Id { get; set; }
    public string Name { get; set; }
    public int Speed { get; set; }
    public int Capacity { get; set; }
    
}