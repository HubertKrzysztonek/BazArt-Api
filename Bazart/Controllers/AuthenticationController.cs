﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Bazart.API.DTO;
using Bazart.API.Services;
using Bazart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Bazart.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthenticationController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }



        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var isUserExist = _userService.CheckIsEmailExist(request.Email);
            if (isUserExist)
            {
                return BadRequest("Email already exist ");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var firstUser = new UserFirstRegistarationDto()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PhoneNumber = request.PhoneNumber,
            };

            _userService.CreateNewUser(firstUser);

            return Ok(firstUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var isUser = _userService.CheckIfUserExist(request);

            if (isUser == false)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, request.Email))
            {
                return BadRequest("Wrong password");
            }

            //var user = _userService.GetUserById() // do rejestracji dodac sprawdzanie email czy juz jest 
            string token = CreateToken(request);

            return Ok(token);

        }

        private string CreateToken(UserLoginDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email , user.Email),
                //new Claim(ClaimTypes.Name , user.FirstName),
                //new Claim(ClaimTypes.MobilePhone , user.PhoneNumber),

            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires:DateTime.Now.AddMinutes(20),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password,string userEmail)
        {
            var userSalt = _userService.GetPasswordSaltByUserEmail(userEmail);
            var userHash = _userService.GetPasswordHashByUserEmail(userEmail);
            using (var hmac = new HMACSHA512(userSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(userHash);
            };
        }

    }
}
