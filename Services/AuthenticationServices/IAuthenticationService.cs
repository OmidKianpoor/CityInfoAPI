using CityInfo.API.Entities;

namespace CityInfo.API.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        public Task<User> Login(string username, string password);
        public Task<User> SignUp(User user);
    }
}
