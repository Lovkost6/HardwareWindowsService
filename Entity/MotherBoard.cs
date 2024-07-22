using System.ComponentModel.DataAnnotations.Schema;

namespace HardWareMonitorService.Entity;
[Table("MotherBoard")]
public class MotherBoard
{
    // public long Id { get; set; }
    public string Name { get; set; }
}