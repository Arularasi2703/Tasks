using AccountAPI.Data;
using FoodOrderingSystemAPI.Data.Repositories;
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using FoodOrderingSystemAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderCartServiceAPI.Repositories;
using System.Text;
using Serilog;
using FoodMenuApi.Services;
using FoodOrderingSystemAPI.Constraints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT authentication
var configuration = builder.Configuration;
var jwtSecretKey = configuration["JwtSettings:SecretKey"];
var jwtIssuer = configuration["JwtSettings:Issuer"];
var jwtAudience = configuration["JwtSettings:Audience"];
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidIssuer = jwtIssuer,
    ValidAudience = jwtAudience,
    ValidateLifetime = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey))
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true for production
    options.SaveToken = true;
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddDbContext<FoodAppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFoodMenuService, MenuService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
builder.Services.Configure<OtpGenerationSettings>(configuration.GetSection("AppSettings:OtpGeneration"));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("category", typeof(CategoryRouteConstraint));
    options.ConstraintMap.Add("handler", typeof(CategoryRouteHandler));
});


builder.Services.AddCors();
string logFileLocation = configuration["Logging:LogFileLocation"];
var _logger = new LoggerConfiguration()
.WriteTo.File(logFileLocation,rollingInterval:RollingInterval.Day)
.CreateLogger();

builder.Logging.AddSerilog(_logger);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Enable authentication
app.UseAuthentication();

app.UseAuthorization();
app.UseSession();
app.UseCors(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
app.MapControllers();

app.Run();
