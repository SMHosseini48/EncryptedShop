﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using ncorep.Models;

namespace MultiLevelEncryptedEshop.Models;

public partial class Order : BaseEntity
{

    public string UserId { get; set; }
    
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; }
}