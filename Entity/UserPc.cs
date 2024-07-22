using System.ComponentModel.DataAnnotations.Schema;
using HardWareMonitorService.Entity;

namespace HardWareMonitorService.Entity;
[Table("UserPc")]
public class UserPc
{
    public string Id { get; set; }
    
    public Cpu Cpu { get; set; }
    
    public MotherBoard MotherBoard { get; set; }
    
    public Gpu Gpu { get; set; }
    
    public OperationSystem Os { get; set; }

    public List<Storage> Storages { get; set; } = new();
    public List<Ram> Rams { get; set; } = [];
}