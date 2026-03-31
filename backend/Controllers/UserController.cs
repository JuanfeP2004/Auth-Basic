using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("authapp/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly AuthDBContext _context;
    private const int minimum_lenght = 8;
    private const int salt_length = 16;

    public UserController(AuthDBContext context)
    {
        _context = context;
    }

    [HttpPost("")]
    public async Task<ActionResult> Create([FromBody] UserTemplate user_template)
    {
        try
        {
            if(user_template == null)
            return BadRequest();

            if(!Utility.ValidateString(user_template.name))
                return BadRequest();
            if(!Utility.ValidateString(user_template.email))
                return BadRequest();
            if(!Utility.ValidateString(user_template.password))
                return BadRequest();
            if(!Utility.ValidateString(user_template.phone))
                return BadRequest();
            if(!Utility.ValidateString(user_template.second_factor))
                return BadRequest();

            if(user_template.password.Length < minimum_lenght)
                return BadRequest("Password too short");

            Factor? factor = await _context.Factors.SingleOrDefaultAsync(
                p => user_template.second_factor.ToUpper() == p.factor_name.ToUpper()
                );
            if(factor == null)
                return BadRequest();

            User? duplicated_user = await _context.Users.SingleOrDefaultAsync(p => p.name_user == user_template.name);
            if (duplicated_user != null)
                return BadRequest();

            string salt = Utility.GenerateSafeString(salt_length);
            System.Console.WriteLine(salt);
            string password = Utility.Sha256Encrypt(user_template.password, salt);
            System.Console.WriteLine(password);

            var new_user = new User
            {
                name_user = user_template.name,
                email = user_template.email,
                phone = user_template.phone,
                password_hash = user_template.password,
                second_auth = factor.factor_id
            };

            //await _context.Users.AddAsync(new_user);
            //await _context.SaveChangesAsync();

            return Created();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.ToString());
            return StatusCode(500);
        }
    }
}