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
    private const string email_pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private const string phone_pattern = @"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

    public UserController(AuthDBContext context)
    {
        _context = context;
    }

    [HttpPost("")]
    public async Task<ActionResult> Create([FromBody] UserTemplate? user_template)
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
                return BadRequest("Password too short");

            if(!Regex.IsMatch(user_template.email, email_pattern))
                return BadRequest("Bad format email");
            else if(!Regex.IsMatch(user_template.phone, phone_pattern))
                return BadRequest("Bad format phone");
            
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
            string password = Utility.Sha256Encrypt(user_template.password, salt);

            var rows_affected = await _context.Database.ExecuteSqlAsync(
                $"INSERT dbo.Users (name_user, email, phone, password_hash, salt_char, second_auth) VALUES ({user_template.name}, {user_template.email}, {user_template.phone}, {password}, {salt}, {factor.factor_id})");
            
            if(rows_affected == 1)
                await _context.SaveChangesAsync();
            else throw new Exception("Something went worng");

            return Created();
        }
        catch
        {
            return StatusCode(500);
        }
    }
}