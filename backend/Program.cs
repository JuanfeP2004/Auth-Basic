using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Connection string
var connectionString = builder.Configuration.GetConnectionString("SqlContext");

// Add services to the container.
builder.Services.AddDbContext<AuthDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<EmailContext>();
builder.Services.AddSingleton<AuthBasic>();
builder.Services.AddScoped<UtilityService>();

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();