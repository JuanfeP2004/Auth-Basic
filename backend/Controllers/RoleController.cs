using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("authapp/v1/[controller]")]
public class RoleController : ControllerBase
{
    AuthService _authService;
    public RoleController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("role1/{uuid:Guid}")]
    public async Task<IActionResult> DoRoleOne(Guid? uuid)
    {
        string role_required = "Role1";
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            UserToken? userToken = await _authService.AuthenticateUser(uuid, token_code);
            if(userToken is null)
                return StatusCode(401, new StringResponse{ Text = "You're not authenticated"});
            if(await _authService.AuthorizeUser(userToken, role_required) is false)
                return StatusCode(403, new StringResponse{ Text = "Action Denegate"});
            
            return Ok(new StringResponse {Text = "You did Action A"});
        }
        catch
        {
            return StatusCode(500);
        }
    }
}