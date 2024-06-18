using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiLevelEncryptedEshop.Dtos;
using MultiLevelEncryptedEshop.Interfaces.Services;

namespace MultiLevelEncryptedEshop.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{

    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterAccountModel registerAccountModel)
    {
        var result = await _accountService.Register(registerAccountModel);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var result = await _accountService.Login(loginModel);
        return Ok(result) ;
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult test()
    {
        return Ok("everything is good");
    }
}