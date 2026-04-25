using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("authapp/v1/[controller]")]
public class LoginTwoController : ControllerBase
{

    LoginService _loginService;    
    public LoginTwoController(LoginService loginService)
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

    /*
    [HttpPost("sendresetcode")]
    public async Task<ActionResult> CreateResetCode([FromBody] OneText body)
    {
        try
        {
            if(body == null)
                return BadRequest();
            if(!Utility.ValidateString(body.Text))
                return BadRequest(new OneText() { Text = "Isn't a code"});

            User? user = await _context.Users.FirstOrDefaultAsync(p => p.email == body.Text);

            if(user == null)
                return NotFound();          

            string code = Utility.GenerateSafeCode(6);

            ResetCode new_code = new ResetCode()
            {
                user_id = user.user_id,
                code = code,
                expires = Utility.AddTime(code_time)
            };

            string subject = "Reset code";
            string message = $"Your code to reset the password in auth basic is: {code}";

            await _context.UserResetCodes.AddAsync(new_code);
            await _context.SaveChangesAsync();

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
        catch(Exception e)
        {
            System.Console.WriteLine(e);
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
            if(!Utility.ValidateString(body.code))
                return BadRequest(new OneText() { Text = "Code isn't present"});
            if(!Utility.ValidateString(body.password))
                return BadRequest(new OneText(){Text="Password isn't present"});
            if(body.password.Length < minimum_lenght)
                return BadRequest(new OneText() {Text = "Password is too short"});

            User? user = await _context.Users.FirstOrDefaultAsync(p => p.uuid == body.guid);

            if(user == null)
                return NotFound();

            ResetCode? reset_code = await _context.UserResetCodes.FirstOrDefaultAsync(
                p => p.code == body.code && p.user_id == user.user_id
                && !p.used && p.expires < DateTime.Now);

            if (reset_code == null)
                return NotFound();

            string password_hash = Utility.Sha256Encrypt(body.password, user.salt_char);

            await _context.Users.Where(p => p.user_id == user.user_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.password_hash, e => password_hash));
            await _context.UserResetCodes.Where(p => p.code_id == reset_code.code_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.used, e => true));
            await _context.SaveChangesAsync();

            return Ok(new OneText(){Text="Password has been changed"});
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e);
            return StatusCode(500);
        }
    }*/
}