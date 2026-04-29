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
    public async Task<User?> FindUser(Guid? uuid)
    {
        try
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.uuid == uuid);
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
    public async Task ModifyPassword(int user_id, string? new_password)
    {
        try {
            await _context.Users.Where(p => p.user_id == user_id)
                .ExecuteUpdateAsync(s => s.SetProperty(s => s.password_hash, e => new_password));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }
    public async Task ModifyIsActive(int user_id, bool is_active)
    {
        try {
            await _context.Users.Where(p => p.user_id == user_id)
                .ExecuteUpdateAsync(s => s.SetProperty(s => s.is_active, e => is_active));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task ModifySecondFactor(int user_id, int factor_id)
    {
        try {
            await _context.Users.Where(p => p.user_id == user_id)
                .ExecuteUpdateAsync(s => s.SetProperty(s => s.second_auth, e => factor_id));
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception();
        }
    }
}