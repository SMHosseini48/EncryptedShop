﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using ncorep.Models;

namespace MultiLevelEncryptedEshop.Models;

public partial class ProductMedia : BaseEntity
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int MediaNumber { get; set; }

    public string Link { get; set; }
    public string ProductId { get; set; }

    public virtual Product Product { get; set; }
    
    public string StoredFileId { get; set; }
    
    public StoredFile StoredFile { get; set; }
}