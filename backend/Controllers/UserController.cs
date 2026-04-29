using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[EnableRateLimiting("applimiter")]
[Route("authapp/v1/[controller]")]
public class UserController : ControllerBase
{
    UserService _userService;
    AuthService _authService;

    public UserController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UserBody? user_template)
    {
        try
        {
            if(user_template == null)
                return BadRequest();
            (int, AuthResponse) response = await _userService.Create(user_template);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPut("changepassword")]
    public async Task<ActionResult> ChangePassword([FromBody] SendCodeBody body)
    {
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            UserToken? userToken = await _authService.AuthenticateUser(body.Uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            
            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _userService.ChangePassword(body.Uuid, body.Code);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPut("change2fa")]
    public async Task<ActionResult> Change2FA([FromBody] SendCodeBody body)
    {
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            UserToken? userToken = await _authService.AuthenticateUser(body.Uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            
            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _userService.Change2FA(userToken.user_id, body.Code);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }


    [HttpPut("lockuser")]
    public async Task<ActionResult> LockUser([FromBody] DoubleGuidBody body)
    {
        string role_required = "LockUser";
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            UserToken? userToken = await _authService.AuthenticateUser(body.Uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            if(await _authService.AuthorizeUser(userToken, role_required) is false)
                return StatusCode(403, new StringResponse { Text = "Action Denegated"});

            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _userService.LockUser(body.Tarjet);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }

     [HttpPut("unlockuser")]
    public async Task<ActionResult> UnlockUser([FromBody] DoubleGuidBody body)
    {
        string role_required = "UnlockUser";
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            UserToken? userToken = await _authService.AuthenticateUser(body.Uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            if(await _authService.AuthorizeUser(userToken, role_required) is false)
                return StatusCode(403, new StringResponse { Text = "Action Denegated"});

            if(body == null)
                return BadRequest();
            (int, AuthResponse) response = await _userService.UnlockUser(body.Tarjet);
            return StatusCode(response.Item1, response.Item2);
        }
        catch
        {
            return StatusCode(500);
        }
    }
}