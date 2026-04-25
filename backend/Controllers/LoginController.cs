using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("authapp/v1/[controller]")]
public class LoginController : ControllerBase
{

    LoginService _loginService;    
    public LoginController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login([FromBody] Credentials body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            
            (int, AuthResponse) response = await _loginService.Login(body);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }  
    
    [HttpPost("send2fa")]
    public async Task<ActionResult> Send2FA([FromBody] SendCodeBody body)
    {
        try
        {          
            if(body == null)
                return BadRequest();
            
            (int, AuthResponse) response = await _loginService.Send2FA(body);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }
    
    [HttpDelete("logout/{uuid:Guid}")]
    public async Task<ActionResult> Logout(Guid? uuid)
    {
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            (int, AuthResponse) response = await _loginService.Logout(uuid, token_code);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("sendresetcode")]
    public async Task<ActionResult> CreateResetCode([FromBody] StringBody body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _loginService.CreateResetCode(body);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }
    
    [HttpPost("resetpassword")]
    public async Task<ActionResult> SendResetCode([FromBody] ResetBody body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _loginService.SendResetCode(body);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }
}