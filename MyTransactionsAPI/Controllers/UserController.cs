using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTransactionsAPI.Data;
using MyTransactionsAPI.Models.Dtos.User;

namespace MyTransactionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TransactionDbContext dbContext;

        public UserController(TransactionDbContext transactionDbContext) {
            dbContext = transactionDbContext;
        }

        // GET      : Get All Users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Data list mapped to User modal
            var users = await dbContext.Users.Where(u => u.Status == "ACTIVE").ToListAsync();
            if(users.Count < 1)
            {
                return NotFound("No records available");
            }
            // Map data to User Dto
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto() { 
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                });
            }

            return Ok(userDtos);

        }

        // GET      : Get User by Id
        [HttpGet]
        [Route("{Id:guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
        {
            // Data fetched matching to User modal
            var User = await dbContext.Users.FindAsync(Id);
            if (User == null)
            {
                return NotFound("Invalid Id or User does not exist");
            }
            if(User.Status == "INACTIVE")
            {
                return NotFound("User does not exist");
            }
            var userDto = new UserDto
            {
                Id = User.Id,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                
            };
            return Ok(userDto);
        }

        [HttpGet]
        [Route("{email}")]
        public async Task<IActionResult> IsUserExits([FromRoute] String email)
        {
            // Data fetched matching to User modal
            var User = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Status == "ACTIVE");
            if (User == null)
            {
                return Ok(false);
            }
            return Ok(true);
        }


        // PUT   : Inactivate user
        [HttpPut]
        [Route("{Id:guid}"), Authorize]
        public async Task<IActionResult> DeactivateUserById([FromRoute] Guid Id)
        {
            var User = await dbContext.Users.FindAsync(Id);
            if (User == null)
            {
                return NotFound("Invalid Id or User does not exist");
            }
            User.Status = "INACTIVE";
            await dbContext.SaveChangesAsync();

            return Ok("Account deleted successfully");
        }

    }
}
