using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTransactionsAPI.Data;
using MyTransactionsAPI.Models.Domains;
using MyTransactionsAPI.Models.Dtos.TransactionType;

namespace MyTransactionsAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypeController : ControllerBase
    {

        private readonly TransactionDbContext dbContext;
        private readonly IMapper _mapper;

        public TransactionTypeController(TransactionDbContext transactionDbContext, IMapper mapper)
        {
            dbContext = transactionDbContext;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult> Get() {
            try
            {
                var transactionsTypes = await dbContext.TransactionTypes.ToListAsync();
                var formattedList = _mapper.Map<TransactionTypeDto[]>(transactionsTypes);
                return Ok(formattedList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(TransactionTypeRequest requestBody)
        {
            try
            {
                var transactionType = new TransactionType
                {
                    Code = requestBody.Code,
                    Name = requestBody.Name,
                };
                await dbContext.TransactionTypes.AddAsync(transactionType);
                await dbContext.SaveChangesAsync();
                return Ok("Transaction type added");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
