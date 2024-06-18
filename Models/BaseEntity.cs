using System;

namespace ncorep.Models;

public abstract class BaseEntity 
{
    public BaseEntity()
    {
        Id = Guid.NewGuid().ToString("N");
        CreatedOn = DateTime.Now;
        ModifiedOn = DateTime.Now;
        Deletable = true;
    }

    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public bool Deletable { get; set; }
}