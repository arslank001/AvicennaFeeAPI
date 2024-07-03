using AvicennaFeeAPI.Data;
using AvicennaFeeAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the HealthCheckService
builder.Services.AddTransient<IHealthCheckService, HealthCheckService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Usage
//var secretKey1 = SecretKeyGenerator.GenerateSecretKey();
//Console.WriteLine(secretKey1);


// Retrieve the secret key from environment variables
//var secretKey = "J6lSRyYu2wT9l4Ol4JoKmush1Oy+TM8Kp3jEccexCBI=";

var secretKey =  builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
	throw new InvalidOperationException("JWT_SECRET_KEY is empty");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddSingleton<ITokenService>(new TokenService(secretKey));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false,
	};
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
