using System.Reflection.Metadata.Ecma335;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//Connection string
var connectionString = builder.Configuration.GetConnectionString("SqlContext");
RateLimiterOptions limiter = new RateLimiterOptions();

// Add services to the container.
builder.Services.AddDbContext<AuthDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<EmailContext>();

builder.Services.AddScoped<UtilityService>();
builder.Services.AddRateLimiter(limiter =>
{
    limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    limiter.AddFixedWindowLimiter("authlimiter", options =>
    {
        options.PermitLimit = 3;
        options.QueueLimit = 0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.Window = TimeSpan.FromMinutes(1);
    });
        limiter.AddFixedWindowLimiter("applimiter", options =>
    {
        options.PermitLimit = 10;
        options.QueueLimit = 0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.Window = TimeSpan.FromMinutes(1);
    });
});

builder.Services.AddScoped<IAuth, AuthRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ILogin, LoginRepository>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();