using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly CityInfoDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthenticationService( CityInfoDbContext dbContext, IConfiguration configuration)
        {
           _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<User> Login(string username, string password)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u=> u.UserName == username);
            if (user == null || password != user.Password)
            { return null; }


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.GivenName, user.FirstName)

                }),
                IssuedAt = DateTime.UtcNow,
                Issuer = _configuration["Authentication:Issuer"],
                Audience = _configuration["Authentication:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.IsActive = true;

            return user;

        }

        public async Task<User> SignUp(User user)
        {
            if (_dbContext.Users.Any(p => p.UserName == user.UserName))
            {
                return null;
            }
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }
    }
}
