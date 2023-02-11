using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _objDataContext;
        private readonly ITokenService _objITokenService;

        //public AccountController(DataContext objDataContext)
        public AccountController(DataContext objDataContext, ITokenService objITokenService)
        {
            _objDataContext = objDataContext;
            _objITokenService = objITokenService;
        }

        [HttpPost("register")]      //URL: api/account/register
        //public async Task<ActionResult<AppUser>> Register(RegisterDTO objRegisterDTO) 
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO objRegisterDTO) 
        {
            if(await UserExists(objRegisterDTO.UserName)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();  // random generated key used as password salt

            var user = new AppUser
            {
                UserName = objRegisterDTO.UserName.ToLower(),
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(objRegisterDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _objDataContext.Add(user);
            await _objDataContext.SaveChangesAsync();

            //return user;
            // because we need a JSON WEB TOKEN after register request call
            return new UserDTO{
                Username = user.UserName,
                Token = _objITokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        //public async Task<ActionResult<AppUser>> Login(LoginDTO objLoginDTO)
        public async Task<ActionResult<UserDTO>> Login(LoginDTO objLoginDTO)
        {
            var user = await _objDataContext.Users.SingleOrDefaultAsync(x => 
            x.UserName == objLoginDTO.Username);

            if(user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt); // this will get exact hashing algorithm used earlier with help of key i.e user.passwordSalt

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(objLoginDTO.Password));// gives hash of user entered password

            // now compare both hashed passwords
            for(int i=0; i<computedHash.Length; i++)
            {
                if(computedHash[i] != user.Password[i]) return Unauthorized("Invalid password");
            }

            //return user;
             // because we need a JSON WEB TOKEN after register request call
            return new UserDTO{
                Username = user.UserName,
                Token = _objITokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _objDataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}