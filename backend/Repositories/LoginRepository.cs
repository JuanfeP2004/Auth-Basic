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

    public async Task<User?> FindUser(string? email)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.email == email && p.is_active);
            if(user == null) return null;
            return user;           
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task<User?> FindUser(Guid? uuid)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.uuid == uuid && p.is_active);
            if(user == null) return null;
            return user;           
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task Create2FACode(int id, string? code, DateTime expires)
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
            throw new Exception();
        }
    }
    public async Task<bool> Use2FACode(int user_id, string? code)
    {
        try {
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
        catch
        {
            throw new Exception();
        }
    }

    public async Task CreateToken(int user_id, string? token_hash, DateTime expires)
    {
        try {
            UserToken new_token = new UserToken()
                {
                    user_id = user_id,
                    token_hash = token_hash,
                    expires = expires
                };

            await _context.UserTokens.AddAsync(new_token);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task RevokeToken(int token_id)
    {
        try
        {
            await _context.UserTokens.Where(p => p.token_id == token_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.revoked, e => true));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task CreateResetCode(int user_id, string? code, DateTime expires)
    {
        try
        {
            await _context.UserResetCodes.AddAsync(new ResetCode
            {
                user_id = user_id,
                code = code,
                expires = expires
            });
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }
    public async Task<bool> UseResetCode(int user_id, string? code)
    {
        try
        {
            ResetCode? reset = await _context.UserResetCodes.FirstOrDefaultAsync(
                p => p.code == code && p.user_id == user_id
                && !p.used && p.expires < DateTime.Now);
        
            if(reset is null)
                return false;
        
            await _context.UserResetCodes.Where(p => p.code_id == reset.code_id).
                ExecuteUpdateAsync(s => s.SetProperty(e => e.used, e => true));
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task ResetPassword(int user_id, string? password_hash)
    {
        try
        {
            await _context.Users.Where(p => p.user_id == user_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.password_hash, e => password_hash));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

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