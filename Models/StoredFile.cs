using System.ComponentModel.DataAnnotations.Schema;
using MultiLevelEncryptedEshop.Enums;
using ncorep.Models;

namespace MultiLevelEncryptedEshop.Models;

public class StoredFile : BaseEntity
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Path { get; set; }
    [Column(TypeName = "decimal(10, 5)")]
    public decimal SizeMB { get; set; }
    public FileType Type { get; set; }
    public string Extension { get; set; }
    public bool IsTemp { get; set; }
}