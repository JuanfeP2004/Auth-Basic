using Microsoft.EntityFrameworkCore;

public class AuthDBContext : DbContext
{
    public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
    {       
    }

    public DbSet<User> Users {get; set;}
    public DbSet<UserRoles> UsersRoles {get; set;}
    public DbSet<Log> Logs {get; set;}

    public DbSet<Factor> Factors {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("Users").HasNoKey();
    }
}