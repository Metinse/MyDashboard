using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyBlog.Entities;
using MyBlog.DataAccess.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using MyBlog.Entities.DTOs;

namespace MyBlog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public LoginController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Kullanıcı girişi yapar.
        /// </summary>
        /// <param name="login">Kullanıcı giriş bilgileri</param>
        /// <returns>Başarılı giriş durumunda JWT token döner</returns>
        [SwaggerOperation(Summary = "Kullanıcı girişi yapar", Description = "Bu endpoint, kullanıcının sisteme giriş yapmasını sağlar ve başarılı giriş durumunda JWT token döner.")]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            // FluentValidation burada devreye girecek, ModelState otomatik olarak doğrulanmış olacak
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // FluentValidation hatalarını döner
            }

            var user = _userRepository.GetUserByUsernameAndPassword(login.UserName, login.Password);

            if (user == null)
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
