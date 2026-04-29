using Microsoft.EntityFrameworkCore;

public class AuthRepository : IAuth
{
    AuthDBContext _context;

    public AuthRepository(AuthDBContext context)
    {
        _context = context;
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
    public async Task<UserToken?> FindToken(int user_id, string? token_hash)
    {
        try
        {
            UserToken? token = await _context.UserTokens.SingleOrDefaultAsync(p => p.token_hash == token_hash 
                && p.user_id == user_id && DateTime.Now < p.expires.AddHours(5) && !p.revoked);
            
            if(token == null) return null;
            return token;
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task RefreshToken(int token_id, DateTime refresh)
    {
        try {
            await _context.UserTokens.Where(p => p.token_id == token_id).
                ExecuteUpdateAsync(s => s.SetProperty(e => e.expires, e => refresh));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task<bool> FindUserRole(int user_id, string? role_name)
    {
        try
        {
            Role? role = await _context.Roles.SingleOrDefaultAsync(p => 
                p.role_name.ToLower() == role_name.ToLower());
            if(role is null) return false;
            
            UserRole? permission = await _context.UserRoles.SingleOrDefaultAsync(p => 
                p.user_id == user_id && p.role_id == role.role_id);
            if(permission is null) return false;

            return true;
        }
        catch
        {
            throw new Exception();
        }
    }
}