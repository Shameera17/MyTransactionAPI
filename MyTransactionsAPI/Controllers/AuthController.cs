using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyTransactionsAPI.Data;
using MyTransactionsAPI.Models.Domains;
using MyTransactionsAPI.Models.Dtos.User;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace MyTransactionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TransactionDbContext dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(TransactionDbContext transactionDbContext, IConfiguration configuration, IMapper mapper)
        {
            dbContext = transactionDbContext;
            _configuration = configuration;
            _mapper = mapper;
        }



        // POST     : Create user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            // check if user exists
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == userDto.Email && user.Status == "ACTIVE");

            if (user != null)
            {
                return BadRequest("User already exists");
            }

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var User = new User
            {
                Email =  userDto.Email,
                FirstName =  userDto.FirstName,
                LastName =  userDto.LastName,
                Status = "ACTIVE",
                PasswordHash = passwordHash ,
                PasswordSalt = passwordSalt,
            };

            await dbContext.Users.AddAsync(User);

            await dbContext.SaveChangesAsync();

           var _user = new UserViewRequest
            {
                FirstName = User.FirstName,
                LastName = User.LastName,
                Status = User.Status,
                Email = User.Email,
            };

            return Ok(_user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            // check if user exists
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == loginRequest.Email && user.Status == "ACTIVE");
            if (user == null)
            {
                return NotFound("User not found");
            }
            var flag = VerifyPasswordHash(loginRequest.Password, user.PasswordHash, user.PasswordSalt);
            if(flag == false)
            {
                return BadRequest("Password or Email is incorrect!");
            }
            string token = CreateToken(user).ToString();
            
            var content = new
            {
                user = _mapper.Map<UserViewRequest>(user),
                token = token
            };
            return Ok(content);
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(LoginRequest loginRequest)
        {
            // check if user exists and user is active
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == loginRequest.Email && user.Status == "ACTIVE");
            if (user == null)
            {
                return NotFound("Entered email is invalid");
            }
            else if (user.Status == "INACTIVE")
            {
                return BadRequest("User does not exist");
            }
            CreatePasswordHash(loginRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            dbContext.SaveChanges();

            return Ok("Password reset successfully");
        }


        [HttpPut("updatePassword"),Authorize]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest requestBody)
        {
            // check if user exists and user is active
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == requestBody.Email && user.Status == "ACTIVE" && user.Id == requestBody.Id);
            if (user == null)
            {
                return NotFound("Entered email is invalid");
            }
            else if (user.Status == "INACTIVE")
            {
                return BadRequest("User does not exist");
            }
            var flag = VerifyPasswordHash(requestBody.OldPassword, user.PasswordHash, user.PasswordSalt);
            if(flag)
            {
                CreatePasswordHash(requestBody.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;


                await dbContext.SaveChangesAsync();

                

                return Ok("Password reset successfully");

            }

            return NotFound("Old password is incorrect!");


        }

        [HttpPut("updateUser"), Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest requestBody)
        {
            // check if user exists and user is active
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Status == "ACTIVE" && user.Id == requestBody.Id);
            if (user == null)
            {
                return NotFound("Entered email is invalid");
            }
            else if (user.Status == "INACTIVE")
            {
                return BadRequest("User does not exist");
            }
           

            user.FirstName = requestBody.FirstName;
            user.LastName = requestBody.LastName;
            user.Email = requestBody.Email;

            await dbContext.SaveChangesAsync();

            string token = CreateToken(user).ToString();

            var content = new
            {
                user = _mapper.Map<UserViewRequest>(user),
                token = token
            };
            return Ok(content);


        }



        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt )
        {
                using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(
            string password, 
            byte[] passwordHash, 
            byte[] passwordSalt
            )
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            //claims ? properties that describe logged in user
            List<Claim> claims = new List<Claim>
            {
                new Claim("name",ClaimTypes.Name),
                new Claim("Email",user.Email),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
                new Claim("Id",user.Id.ToString()),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSetting:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
               claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: cred
               
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
