using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using API.DTO;
using Microsoft.EntityFrameworkCore;
using API.Interface;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registrationDto)
        {
            if (await UserExists(registrationDto.userName)) return BadRequest("UserName is Taken ");

            var hmac = new HMACSHA512();
            var user = new AppUser
            {
                userName = registrationDto.userName.ToLower(),
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDto.password)),
                passwordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                username = user.userName,
                token = _tokenService.CreateToken(user)
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.userName == loginDto.UserName);
            if (user == null) return BadRequest("Invalid User");

            var hmac = new HMACSHA512(user.passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.passwordSalt[i]) return BadRequest("Invalid password");
            }
            return new UserDto
            {
                username = user.userName,
                token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.userName == userName.ToLower());
        }
    }
}