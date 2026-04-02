using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("authapp/v1/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AuthDBContext _context;

    public LoginController(AuthDBContext context)
    {
        _context = context;
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login([FromBody] Credentials body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            if(!Utility.ValidateString(body.email))
                return BadRequest();
            if(!Utility.ValidateString(body.password))
                return BadRequest();

            User? user = await _context.Users.SingleOrDefaultAsync(p => p.email == body.email);
            if(user == null) return NotFound();

            if(!user.is_active) return NotFound();

            if(!Utility.Sha256Encrypt(body.password, user.salt_char).Equals(user.password_hash))
                return NotFound();

            return Ok(user); 
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("send2fa")]
    public async Task<ActionResult> SecondFactor()
    {
        return BadRequest();
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        return BadRequest();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh()
    {
        return BadRequest();
    }

    string SendSMS()
    {
        return "";
    }

    string SendEmail()
    {
        return "";
    }
}