using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NGK_11.Data;
using NGK_11.Models;
using NGK_11.Utilities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BCrypt.Net.BCrypt;

namespace NGK_11.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        const int BcryptWorkfactor = 10;

        public AccountController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }


        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<TokenDto>> RegisterFromBody(UserDto regUser)
        {

            var (token,user) = await RegisterUser(regUser);
            if(token == null || user == null)
            {
                return BadRequest(new { errorMessage = "Email already in use" });
            }
            return CreatedAtAction("Get", new { id = user.UserId }, token);
        }

        [HttpPost("registerFromForm"), AllowAnonymous]
        public async Task<ActionResult<TokenDto>> RegisterFromForm([FromForm]UserDto regUser)
        {
            var (token, user) = await RegisterUser(regUser);
            if (token == null || user == null)
            {
                return BadRequest(new { errorMessage = "Email already in use" });
            }
            
            return NoContent();
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<TokenDto>> Login(UserDto login)
        {
            login.Email = login.Email.ToLower();
            var user = await _context.Users.Where(u => u.Email == login.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                var validPwd = Verify(login.Password, user.PwHash);
                if (validPwd)
                {
                    var token = new TokenDto
                    {
                        JWT = GenerateToken(user)
                    };
                    return token;
                }
            }
            ModelState.AddModelError(string.Empty, "Forkert brugernavn eller password");
            return BadRequest(ModelState);
        }


        // GET: api/Account/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var userDto = new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return userDto;
        }

        private string GenerateToken(User user)
        {
            
            var claims = new Claim[]
            {
                new Claim("Email", user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),     
                // roleClaim,
                new Claim("UserId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
            var token = new JwtSecurityToken(
                 new JwtHeader(new SigningCredentials(
                      new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)),
                      new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<(TokenDto,User)> RegisterUser(UserDto regUser)
        {
            regUser.Email = regUser.Email.ToLower();
            var emailExist = await _context.Users.Where(u => u.Email == regUser.Email).FirstOrDefaultAsync();
            if (emailExist != null)
                return (null,null);
            User user = new()
            {
                Email = regUser.Email,
                FirstName = regUser.FirstName,
                LastName = regUser.LastName
            };
            user.PwHash = HashPassword(regUser.Password, BcryptWorkfactor);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var token = new TokenDto
            {
                JWT = GenerateToken(user)
            };
            
            return (token,user);
        }
    }
}
