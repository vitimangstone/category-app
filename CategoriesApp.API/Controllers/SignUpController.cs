using CategoriesApp.API.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CategoriesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly CategoriesContext _context;
        private readonly JWT _jwtConfig;
        public SignUpController(CategoriesContext context, IOptions<JWT> jwtConfig)
        {
            _context = context;
            _jwtConfig = jwtConfig.Value;
        }
        [HttpPost("Register")] 
        public IActionResult Register([FromBody] AuthDetails signUp)
        {
            var user = new User
            {
                UserName = signUp.UserName,
                Password = signUp.Password
            };

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // POST api/<SignUpController>
        [HttpPost("Login")]
        public IActionResult Post([FromBody] AuthDetails signUp)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == signUp.UserName && x.Password == signUp.Password);
            if(user != null)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));

                var token = new JwtSecurityToken(
                    issuer: _jwtConfig.ValidIssuer,
                    audience: _jwtConfig.ValidAudience,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
