using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiLevelEncryptedEshop.Dtos;
using MultiLevelEncryptedEshop.Interfaces.Services;
using MultiLevelEncryptedEshop.Middlewares;
using MultiLevelEncryptedEshop.Models;

namespace MultiLevelEncryptedEshop.Services;

public class AccountService : IAccountService
{
    private readonly MultiLevelEncryptedShopContext _shopContext;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AccountService(MultiLevelEncryptedShopContext shopContext, IConfiguration configuration)
    {
        _shopContext = shopContext;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<SignInInfoDto> Register(RegisterAccountModel registerAccountModel)
    {
        var validationResult = registerAccountModel.Validate();
        if (validationResult.Errors.Count > 0)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _shopContext.Users.FindAsync(registerAccountModel.Email.Normalize());
        if (user != null)
        {
            throw new Exception("user already exists");
        }

        user = new User
        {
            FirstName = registerAccountModel.FirstName,
            LastName = registerAccountModel.LastName,
            Email = registerAccountModel.Email.Normalize(),
            PasswordHash = _passwordHasher.HashPassword(user, registerAccountModel.Password),
            IsAuthenticated = 1
        };

        var userRegisterResult = await _shopContext.Users.AddAsync(user);
        await _shopContext.SaveChangesAsync();
        
        var result = await GenerateToken(userRegisterResult.Entity);

        return new SignInInfoDto {AccessToken = result.AccessToken, RefreshToken = result.RefreshToken};
    }

    public async Task<SignInInfoDto> Login(LoginModel loginModel)
    {
        var validationResult = loginModel.Validate();
        if (validationResult.Errors.Count > 0)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        //check email 
        var user = await _shopContext.Users.FirstOrDefaultAsync(x => x.Email == loginModel.Email.Normalize());
        if (user == null)
        {
            throw new NullReferenceException();
        }
        
        //check password hash 
        var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);
        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException();
        }
        //return token
        return await GenerateToken(user);
    }
    
    private async Task<SignInInfoDto> GenerateToken(User user)
    {
        //create an access token based on returned user info 
        var secret = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
        var accessTokenLifeTime = TimeSpan.Parse(_configuration["JWT:AccessTokenLifeTime"]);
        var refreshTokenLifetime = TimeSpan.Parse(_configuration["JWT:RefreshTokenLifeTime"]);

        byte[] ecKey = new byte[256 / 8];
        Array.Copy(secret, ecKey, 256 / 8);

        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            }),

            
            
            Expires = DateTime.UtcNow.Add(accessTokenLifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(ecKey) , SecurityAlgorithms.Aes256KW ,SecurityAlgorithms.Aes256CbcHmacSha512 )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        //create a refresh token based on user info 
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRandomBytes(128),
            RefreshTokenExpireOn = DateTime.Now.Add(refreshTokenLifetime)
        };

        var refreshTokenRegisterResult = await _shopContext.AddAsync(refreshToken);
        await _shopContext.SaveChangesAsync();
        // return the result
        return new SignInInfoDto {RefreshToken = refreshTokenRegisterResult.Entity.Token, AccessToken = TokenEncryptionMiddleware.Encrypt(accessToken)};
    }

    public static string GenerateRandomBytes(int length = 16, bool urlFrendly = true)
    {
        using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
        {
            byte[] tokenData = new byte[length];
            rng.GetBytes(tokenData);

            string token = Convert.ToBase64String(tokenData);

            if (urlFrendly)
            {
                return token.Replace("/", "-").Replace("+", ".").Replace("=", "_");
            }
            else
            {
                return token;
            }
        }
    }
}