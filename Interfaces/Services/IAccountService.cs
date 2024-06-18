using MultiLevelEncryptedEshop.Dtos;

namespace MultiLevelEncryptedEshop.Interfaces.Services;

public interface IAccountService
{
     Task<SignInInfoDto> Register(RegisterAccountModel registerAccountModel);
     Task<SignInInfoDto> Login(LoginModel loginModel);
}