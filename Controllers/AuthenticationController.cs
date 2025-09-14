using CityInfo.API.Entities;
using CityInfo.API.Models.AuthenticateModels;
using CityInfo.API.Services.AuthenticationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {   private readonly IAuthenticationService _authenticationService;
        
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            
            _authenticationService = authenticationService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationRequestBodyDto authenticationRequestBody)
        {
            if(authenticationRequestBody.UserName == null)
            {
                return BadRequest(new { mesaage = "Please Enter UserName" });
            }
            if(authenticationRequestBody.Password == null)
            {
                return BadRequest(new { mesaage = "Please Enter Password" });
            }
            User? userToLogin = 
                await _authenticationService.Login(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            if (userToLogin != null)
            {
                return Ok($"Hello {userToLogin.FirstName} \n your token is : {userToLogin.Token}");
            
            }
            return NotFound();


        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]CityInfoUsersDto cityInfoUsers)
        {
            if (cityInfoUsers.UserName == null)
            {
                return BadRequest(new { message = "Please Enter UserName" });
            }
            if (cityInfoUsers.Password == null)
            {
                return BadRequest(new { message = "Please Enter Password " });
            }
            if (cityInfoUsers.UserEmail == null)
            {
                return BadRequest(new { message = "Please Enter Email " });
            }
            if (cityInfoUsers.FirstName == null)
            {
                return BadRequest(new { message = "Please Enter FirstName " });
            }
            if (cityInfoUsers.LastName == null)
            {
                return BadRequest(new { message = "Please Enter LastName " });
            }
            if (cityInfoUsers.City == null)
            {
                return BadRequest(new { message = "Please Enter Your City " });
            }
            if (cityInfoUsers.UserPhone == null)
            {
                return BadRequest(new { message = "Please Enter Your Phone Number " });
            }

            User userToSignUp =
                new User(cityInfoUsers.UserName,cityInfoUsers.FirstName,cityInfoUsers.LastName
                ,cityInfoUsers.UserEmail,cityInfoUsers.UserPhone,cityInfoUsers.City,cityInfoUsers.Password);

            User signedUpUser = await _authenticationService.SignUp(userToSignUp);
            if (signedUpUser == null) { return BadRequest(new { message = "Please Chaange UserName" }); }

            User UserToLogin = await _authenticationService.Login(signedUpUser.UserName, signedUpUser.Password);
            if (UserToLogin == null) { return NotFound(); }

            return Ok(UserToLogin);

        }

    }
}
