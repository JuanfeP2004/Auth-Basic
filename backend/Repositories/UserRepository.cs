using Microsoft.EntityFrameworkCore;

public class UserRepository : IUser 
{
    AuthDBContext _context;

    public UserRepository(AuthDBContext context)
    {
        _context = context;
    }


    public async Task<User?> FindUser(string? name, string? email)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.name_user == name
                || p.email == email);
            if (user is null) return null;
            return user;
        }
        catch
        {
            throw new Exception();
        }
    }
    public async Task CreateUser(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task<Factor?> FindAuthFactor(string? text)
    {
        try
        {
            Factor? factor = await _context.Factors.SingleOrDefaultAsync(
                p => text.ToUpper() == p.factor_name.ToUpper());
            if(factor is null) return null;
            return factor;
        }
        catch
        {
            throw new Exception();
        }
    }
}