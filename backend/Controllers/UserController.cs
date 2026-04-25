using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("authapp/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
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
    public async Task<ActionResult> ChangePassword()
    {
        return Ok();
    }
}