﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using ncorep.Models;

namespace MultiLevelEncryptedEshop.Models;

public partial class OrderDetail : BaseEntity
{

    public string OrderId { get; set; }

    public string ProductId { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }
}