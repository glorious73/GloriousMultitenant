using Application.Infrastructure.AppData;
using Application.Logic;
using Application.Logic.Account;
using Application.Logic.Auth;
using Application.Logic.Email;
using Data.UnitOfWork;
using Entity.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using Utility.Email;
using Utility;
using WebAPI.Configuration;
using WebAPI.Middleware;
using WebAPI.Configuration.Db;

var builder = WebApplication.CreateBuilder(args);

string corsName = "CorsName";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsName, policyBuilder => policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.WriteIndented = true;
});

// JWT Authentication
var encryptionKey = RSA.Create(3072);
var signingKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
var secretKid = builder.Configuration["Jwt:SecretKeyId"];
var encryptionKid = builder.Configuration["Jwt:EncryptionKeyId"];
var privateEncryptionKey = new RsaSecurityKey(encryptionKey) { KeyId = encryptionKid };
var publicSigningKey = new ECDsaSecurityKey(ECDsa.Create(signingKey.ExportParameters(false))) { KeyId = secretKid };

builder.Services.AddAuthentication(authoptions =>
{
    authoptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authoptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Get<string>(),
        ValidateIssuer = true,
        ValidateAudience = false,
        IssuerSigningKey = publicSigningKey,
        TokenDecryptionKey = privateEncryptionKey,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

// Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Unit of Work
builder.Services.AddScoped<IUnit, Unit>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Utilities
builder.Services.AddScoped<IEmailUtility, EmailUtility>();
builder.Services.AddScoped<IExpressionUtility, ExpressionUtility>();

// Data
builder.Services.AddSingleton<IAppData, AppData>();
builder.Services.AddScoped<IDbSeedConfig, DbSeedConfig>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", info: new OpenApiInfo { Title = "Glorious MultiTenant", Version = "v1" });
    option.OperationFilter<HeaderFilter>();
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Initialize
app.SeedDb();
app.AssignGlobalData(encryptionKey, signingKey);

// Swagger
var swaggerConfig = new SwaggerConfig();
builder.Configuration.GetSection(nameof(SwaggerConfig)).Bind(swaggerConfig);
app.UseSwagger(option => { option.RouteTemplate = swaggerConfig.JsonRoute; });
app.UseSwaggerUI(option => { option.SwaggerEndpoint(swaggerConfig.UIEndpoint, swaggerConfig.Description); });

app.UseTiming();

app.UseCors(corsName);

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler("/Error");

app.MapControllers();

app.Run();
