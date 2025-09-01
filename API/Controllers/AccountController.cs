using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(AppDBContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")] //api/acount/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await EmailExist(registerDto.Email).ConfigureAwait(false)) return BadRequest("Email taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            await context.Users.AddAsync(user).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return user.ToDto(tokenService);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email).ConfigureAwait(false);

            if (user == null) return Unauthorized("Invalid email adress");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid passwodr!");


            }
            return user.ToDto(tokenService);
        }



        private async Task<bool> EmailExist(string email)
        {
            return await context.Users.AnyAsync(x => x.Email == email).ConfigureAwait(false);  
        }



    }
}
