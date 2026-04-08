using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("authapp/v1/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AuthDBContext _context;
    private readonly EmailContext _email;

    private readonly TimeSpan code_time = new TimeSpan(0, 5, 0);
    private readonly TimeSpan refresh_token_time = new TimeSpan(0, 15, 0);
    private readonly TimeSpan token_time = new TimeSpan(0, 30, 0);
    public LoginController(AuthDBContext context, EmailContext email)
    {
        _context = context;
        _email = email;
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login([FromBody] Credentials body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            if(!Utility.ValidateString(body.email))
                return BadRequest(new Response(){Text="Email isn't present"});
            if(!Utility.ValidateString(body.password))
                return BadRequest(new Response(){Text="Password isn't present"});

            User? user = await _context.Users.SingleOrDefaultAsync(p => p.email == body.email);
            if(user == null) return NotFound(new Response(){Text="Incorrect user"});

            if(!user.is_active) return NotFound(new Response(){Text="User is locked"});

            if(!Utility.Sha256Encrypt(body.password, user.salt_char).Equals(user.password_hash))
                return NotFound(new Response(){Text="Incorrect password"});

            string code = Utility.GenerateSafeCode(6);
            string subject = "Authetication code";
            string message = $"Your code to authenticate in auth basic is: {code}";

            //System.Console.WriteLine(code);
            //System.Console.WriteLine(Utility.AddTime(code_time));

            var rows_affected = await _context.Database.ExecuteSqlAsync(
                $"INSERT dbo.SecondFactorCodes (user_id, code, expires) VALUES ({user.user_id}, {code}, {Utility.AddTime(code_time)})");

            if(rows_affected != 1)
                throw new Exception("Error generating the code");

            switch (user.second_auth)
            {
                case 1:
                    return BadRequest();
                case 2:
                    bool result = await _email.SendEmailAsync(user.email, subject, message);
                    if(!result)
                        throw new Exception("Fail Sending the email");
                    return Ok(new Response(){Text = "It was sent a message to your email with the code, check it."});
                default:
                    throw new Exception("Don't exists an 2fa");
            } 
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
}