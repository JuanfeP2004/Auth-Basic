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
        if(body == null)
            return BadRequest();

        User user = await _context.Users.SingleAsync(p => p.email == body.email);
        if(user == null) return Unauthorized();

        if(!user.is_active) return NotFound();

        if(!Sha256Encrypt(body.password, user.salt_char).Equals(user.password_hash))
            return NotFound();

        return Ok(user);
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

    string Sha256Encrypt(string password, string salt)
    {
        string raw_string = salt + password;
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw_string));
            StringBuilder result = new StringBuilder();
            foreach(byte i in bytes)
                result.Append(i.ToString("x2"));

            return result.ToString();
        }
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