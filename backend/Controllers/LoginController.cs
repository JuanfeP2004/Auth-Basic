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

    private readonly AuthBasic _auth;

    private readonly TimeSpan code_time = new TimeSpan(0, 60, 0);
    private readonly TimeSpan expire_token_time = new TimeSpan(0, 60, 0);
    public LoginController(AuthDBContext context, EmailContext email, AuthBasic auth)
    {
        _context = context;
        _email = email;
        _auth = auth;
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

            var rows_affected = await _context.Database.ExecuteSqlAsync(
                $"INSERT dbo.SecondFactorCodes (user_id, code, expires) VALUES ({user.user_id}, {code}, {Utility.AddTime(code_time)});");

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
                    return Ok(new GuidBody(){guid = user.uuid, text = "It was sent a message to your email with the code, check it."});
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
    public async Task<ActionResult> SecondFactor([FromBody] GuidBody body)
    {
        try
        {          
            if(body == null)
                return BadRequest();
            if(!Utility.ValidateString(body.text))
                return BadRequest(new Response() { Text = "Isn't a code"});

            User? user = await _context.Users.FirstOrDefaultAsync(p => p.uuid == body.guid);

            if(user == null)
                return NotFound();

            SecondFactorCode? factor = await _context.SecondFactorCodes.FirstOrDefaultAsync(
                p => p.code == body.text && p.user_id == user.user_id
                && !p.used && p.expires < DateTime.Now);

            if (factor == null)
                return NotFound();

            string token_code = Utility.GenerateSafeString(24);
            string token_hash = Utility.Sha256Encrypt(token_code);

            UserToken new_token = new UserToken()
            {
                user_id = user.user_id,
                token_hash = token_hash,
                expires = Utility.AddTime(expire_token_time)
            };

            await _context.UserTokens.AddAsync(new_token);
            await _context.SecondFactorCodes.Where(p => p.code_id == factor.code_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.used, e => true));
            await _context.SaveChangesAsync();

            ReturnUser response = new ReturnUser()
            {
                uuid = user.uuid,
                name_user = user.name_user,
                email = user.email,
                phone = user.phone,
                is_active = user.is_active,
                created_at = user.created_at,
                token = token_code
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpDelete("logout/{uuid:Guid}")]
    public async Task<ActionResult> Logout(Guid uuid)
    {
        try
        {
            string? token_code = Request.Headers["AuthToken"];
            if(!Utility.ValidateString(token_code))
                return BadRequest();
            UserToken? token = await _auth.AutheticateUser(_context, token_code, uuid);
            if(token == null)
                return Unauthorized();

            await _context.UserTokens.Where(p => p.token_id == token.token_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.revoked, e => true));
            await _context.SaveChangesAsync();

            return NoContent();       
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e);
            return StatusCode(500);
        }
    }
}