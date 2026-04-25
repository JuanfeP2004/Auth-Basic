using Microsoft.EntityFrameworkCore;

public class AuthDBContext : DbContext
{
    public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
    {       
    }

    public DbSet<User> Users {get; set;}
    public DbSet<Log> Logs {get; set;}
    public DbSet<Factor> Factors {get; set;}
    public DbSet<SecondFactorCode> SecondFactorCodes {get; set;}
    public DbSet<ResetCode> UserResetCodes {get; set;}
    public DbSet<UserToken> UserTokens {get; set;}
    public DbSet<Role> Roles {get; set;}
    public DbSet<UserRole> UserRoles {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserRole>().ToTable("UserRoles").HasNoKey();
    }
}