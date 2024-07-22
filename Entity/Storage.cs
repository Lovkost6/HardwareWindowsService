using System.ComponentModel.DataAnnotations.Schema;

namespace HardWareMonitorService.Entity;
[Table("Storage")]
public class Storage
{
    // public long Id { get; set; }
    public string Name { get; set; }
    
    public int TotalSize { get; set; }
}