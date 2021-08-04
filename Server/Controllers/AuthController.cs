using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Server.Auth;

namespace Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly UserManager<IdentityUser> userManager;
        //private readonly IConfiguration configuration;
        public AuthController(IJwtAuthenticationManager jwtAuthenticationManager, UserManager<IdentityUser> userManager)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.userManager = userManager;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginForm userCredits)
        {
            var user = await userManager.FindByEmailAsync(userCredits.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, userCredits.Password))
            {
                var token = jwtAuthenticationManager.Authenticate(userCredits.Email);
                var result = new UserResponseDTO() { Id = user.Id, UserName = user.UserName, Token = token };
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterForm registrationForm)
        {
            var userExists = await userManager.FindByEmailAsync(registrationForm.Email);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            IdentityUser user = new()
            {
                Email = registrationForm.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registrationForm.Username
            };

            var result = await userManager.CreateAsync(user, registrationForm.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var token = jwtAuthenticationManager.Authenticate(user.Email);
            var newUser = new UserResponseDTO() { Id = user.Id, UserName = user.UserName, Token = token };

            return Ok(newUser);
        }

    }
}
