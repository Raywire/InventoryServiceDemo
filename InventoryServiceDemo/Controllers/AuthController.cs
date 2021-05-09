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
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Post([FromBody] UserLoginRequest _userData)
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
                    return BadRequest(new ErrorResponse() {
                        Errors = new List<string>()
                        {
                            "Invalid credentials"
                        },
                        Success = false
                    });
                }
            }

            return BadRequest(new ErrorResponse() {
                Errors = new List<string>() {
                    "Invalid Payload"
                },
                Success = false
            });
        }

        private async Task<UserInfo> GetUser(string email, string password)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(user => user.Email == email);

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