using Microsoft.EntityFrameworkCore;

public class LoginRepository : ILogin
{

    AuthDBContext _context;
    EmailContext _email;

    public LoginRepository(AuthDBContext context, EmailContext email)
    {
        _email = email;
        _context = context;
    }

    public async Task<User?> FindUser(string email)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.email == email && p.is_active);
            if(user == null) return null;
            return user;           
        }
        catch
        {
            throw new Exception("Something Went Wrong");
        }
    }

    public async Task<User?> FindUser(Guid uuid)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.uuid == uuid && p.is_active);
            if(user == null) return null;
            return user;           
        }
        catch
        {
            throw new Exception("Something Went Wrong");
        }
    }

    public async Task Create2FACode(int id, string code, DateTime expires)
    {
        try 
        {
            await _context.SecondFactorCodes.AddAsync(new SecondFactorCode
            {
                user_id = id,
                code = code,
                expires = expires
            });
            await _context.SaveChangesAsync();
        } catch
        {
            throw new Exception("Something Went Wrong");
        }
    }
    public async Task<bool> Use2FACode(int user_id, string code)
    {
        SecondFactorCode? factor = await _context.SecondFactorCodes.FirstOrDefaultAsync(
            p => p.code == code && p.user_id == user_id
            && !p.used && p.expires < DateTime.Now);
        
        if(factor is null)
            return false;
        
        await _context.SecondFactorCodes.Where(p => p.code_id == factor.code_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.used, e => true));
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task CreateToken(int user_id, string token_hash, DateTime expires)
    {
        UserToken new_token = new UserToken()
            {
                user_id = user_id,
                token_hash = token_hash,
                expires = expires
            };

        await _context.UserTokens.AddAsync(new_token);
        await _context.SaveChangesAsync();
    }
    /*
    void Logout(string token_hash)
    {
        
    }
    string CreateResetCode(string email)
    {
        
    }
    string SendResetCode(string email)
    {
        
    }

    */
    public async Task SendEmail(string email, string subject, string message)
    {
        bool result = await _email.SendEmailAsync(email, subject, message);
        if(!result)
            throw new Exception("Fail Sending the email");
    }
    //async string SendSMS(string phone, string subject, string message)
    //{
        
    //}
}