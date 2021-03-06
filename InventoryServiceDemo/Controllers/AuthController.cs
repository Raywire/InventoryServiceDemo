using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InventoryServiceDemo.DTOs.Requests;
using InventoryServiceDemo.DTOs.Responses;
using InventoryServiceDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryServiceDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly InventoryContext _context;
        private IMapper _mapper;

        public AuthController(IConfiguration config, InventoryContext context, IMapper mapper)
        {
            _configuration = config;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest _userData)
        {
            if(ModelState.IsValid)
            {
                var user = await GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    var jwtToken = GenerateJwtToken(user);

                    return Ok(new LoginResponse()
                    {
                        Success = true,
                        Token = jwtToken,
                        User = _mapper.Map<UserInfoReadDto>(user)
                    });
                }
                else
                {
                    Errors errors = new Errors();
                    errors.Message = "Invalid credentials";
                    return BadRequest(new ErrorResponse() {
                        Errors = errors,
                        Success = false
                    });
                }
            } 
            else
            {
                Errors errors = new Errors();
                errors.Message = "Invalid payload";

                return BadRequest(new ErrorResponse() {
                    Errors = errors,
                    Success = false
                });
            }
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto _userData)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(user => user.Email == _userData.Email || user.UserName == _userData.UserName);
            if (user != null)
            {
                Errors errors = new Errors();
                errors.Message = "User exists";
                return BadRequest(new ErrorResponse()
                {
                    Errors = errors,
                    Success = false
                });
            }

            var userDataModel = _mapper.Map<UserInfo>(_userData);

            userDataModel.Password = BCrypt.Net.BCrypt.HashPassword(_userData.Password);
            _context.UserInfo.Add(userDataModel);
            await _context.SaveChangesAsync();

            return Ok("User has been created");
        }

        private async Task<UserInfo> GetUser(string email, string password)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                return null;
            }

            if (VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        private string GenerateJwtToken(UserInfo user)
        {
            //create claims details based on the user information
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserName", user.UserName),
                    new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        private bool VerifyPassword (string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}