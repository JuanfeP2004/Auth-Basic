using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("authapp/v1/[controller]")]
public class LoginController : ControllerBase
{

    LoginService _loginService;
    AuthService _authService; 
    public LoginController(LoginService loginService, AuthService authService)
    {
        _authService = authService;
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

            UserToken? userToken = await _authService.AuthenticateUser(uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            
            (int, AuthResponse) response = await _loginService.Logout(userToken);
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