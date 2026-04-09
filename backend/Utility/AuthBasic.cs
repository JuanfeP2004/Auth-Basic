using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuthBasic
{

    private readonly TimeSpan refresh_token_time = new TimeSpan(0, 15, 0);
    public AuthBasic()
    {
        
    }

    public async Task<UserToken?> AutheticateUser(AuthDBContext _context, string? token, Guid uuid)
    {
        try
        {           
            if(token == null)
                return null;

            string token_hash = Utility.Sha256Encrypt(token);
            User? user = await _context.Users.FirstOrDefaultAsync(p => p.uuid == uuid);
            UserToken? valid_token = await _context.UserTokens.SingleOrDefaultAsync(
                p => p.token_hash == token_hash && p.user_id == user.user_id 
                && p.expires < DateTime.Now && !p.revoked);

            if(valid_token == null)
                return null;

            DateTime new_expires = DateTime.Now.Add(refresh_token_time);

            await _context.UserTokens.Where(p => p.token_id == valid_token.token_id).
            ExecuteUpdateAsync(s => s.SetProperty(e => e.expires, e => new_expires));
            await _context.SaveChangesAsync();
            return valid_token;
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString());
        }
    }
    //public Task<bool> AuthorizeUser(AuthDBContext _context, string role, Guid uuid)
    //{
        
    //}
}