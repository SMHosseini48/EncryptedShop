using ncorep.Models;

namespace MultiLevelEncryptedEshop.Models;

public class RefreshToken : BaseEntity
{
   
    public string Token { get; set; }
    public DateTime RefreshTokenExpireOn { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}