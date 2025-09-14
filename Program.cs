using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Repositories;
using CityInfo.API.Services.AuthenticationServices;
using CityInfo.API.Services.MailServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// Create Serilog logger
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.Console()
.WriteTo.File("logs/CityInfo.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(option =>
{ 
  
 
    option.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "cityinfoapi", Version = "v1" });

    // Define the Bearer authentication scheme
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token with Bearer prefix (Example: 'Bearer {token}')",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Require Bearer token for all endpoints by default
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
{
{
new OpenApiSecurityScheme
{
Reference = new OpenApiReference
{
Type = ReferenceType.SecurityScheme,
Id = "Bearer"
},
// The following fields help Swagger UI wire up the header correctly
Scheme = "bearer",
Name = "Authorization",
In = ParameterLocation.Header
},
new string[] { }
}
});

});

//builder.Services.AddSwaggerGen();

// Lightweight content type provider and in-memory data store registrations
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#if DEBUG
builder.Services.AddScoped<IMailService, LocalMailService>();
#else
builder.Services.AddScoped<IMailService, CloudMailService>();
#endif

builder.Services.AddDbContext<CityInfoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:CityInfoConnection"]);
});

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

//JWT Bearer authentication configuration
//builder.Services.AddAuthentication("Bearer")
builder.Services.AddAuthentication(opt =>
   {
       opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   })
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Authentication:Audience"]
    };
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Authentication should come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();