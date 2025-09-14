namespace CityInfo.API.Models.AuthenticateModels
{
    public class AuthenticationRequestBodyDto
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }
    }
}
