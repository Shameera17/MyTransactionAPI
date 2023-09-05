using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTransactionsAPI.Data;
using MyTransactionsAPI.Models.Domains;
using MyTransactionsAPI.Models.Dtos.Transaction;
using MyTransactionsAPI.Models.Dtos.TransactionType;
using System;
using System.Collections.Generic;

namespace MyTransactionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionDbContext dbContext;
        private readonly IMapper _mapper;

        public TransactionController(TransactionDbContext transactionDbContext,IMapper mapper)
        {
            dbContext = transactionDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetTransactionByTransactionId([FromQuery] Guid Id)
        {
            try
            {
                var transaction = await (dbContext.Transactions.FirstOrDefaultAsync(item => item.Id == Id));
                
                var formattedList = _mapper.Map<TransactionDto>(transaction);
                return Ok(formattedList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult> GetTransactionByUserId([FromQuery] Guid UserId, int Status, int Year, int Month)
        {
            try
            {
                var query = await dbContext.Transactions
                .Where(transaction => 
                            transaction.UserId == UserId && transaction.Status == Status && transaction.CreatedDate.Year == Year && transaction.CreatedDate.Month == Month) 
                .Join(dbContext.TransactionTypes,
                    transaction => transaction.TransactionTypeId,
                    transactionType => transactionType.Id,
                    (transaction, transactionType) => new
                    {
                        TransactionId = transaction.Id,
                        Amount = transaction.Amount,
                        CreatedDate = transaction.CreatedDate,
                        Name = transaction.Name,
                        Description = transaction.Description,
                        Type = _mapper.Map<TransactionTypeDto>(transactionType)
                    }).OrderByDescending(record => record.CreatedDate)
                .ToListAsync();

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("stats")]
        public async Task<ActionResult> GetTransactionsStats([FromQuery] Guid UserId,  int Year, int Month)
        {
            try
            {

                var _year = 0;
                var _month = 0;

                if(Month == 1) {
                    _month = 12;
                    _year = Year - 1;
                }
                else
                {
                    _month = Month - 1;
                    _year = Year;
                }

                var stats = await dbContext.Transactions
                    .Where(t =>  t.UserId == UserId && t.Status == 1)
                    .GroupBy(t => 1)
                    .Select(g => new
                     {
                        TotalIncome = g
                             .Where(a => a.TransactionType.Code.ToLower() == "income" && (a.CreatedDate.Year < Year || (a.CreatedDate.Year == Year && a.CreatedDate.Month <= Month)))
                             .Sum(t => t.Amount),
                        IncomeCount = g
                         .Where(a => a.TransactionType.Code.ToLower() == "income" && (a.CreatedDate.Year < Year || (a.CreatedDate.Year == Year && a.CreatedDate.Month <= Month)))
                         .Count(),


                        TotalExpenses = g
                             .Where(a => a.TransactionType.Code.ToLower() == "expense" && (a.CreatedDate.Year < Year || (a.CreatedDate.Year == Year && a.CreatedDate.Month <= Month)))
                             .Sum(t => t.Amount),
                        ExpenseCount = g
                             .Where(a => a.TransactionType.Code.ToLower() == "expense" && (a.CreatedDate.Year < Year || (a.CreatedDate.Year == Year && a.CreatedDate.Month <= Month)))
                             .Count(),


                        PreviousIncomes = g
                             .Where(a => a.TransactionType.Code.ToLower() == "income" && a.CreatedDate.Year == _year && a.CreatedDate.Month == _month)
                             .Sum(t => t.Amount),
                        PreviousIncomeCount = g
                         .Where(a => a.TransactionType.Code.ToLower() == "income" && a.CreatedDate.Year == _year && a.CreatedDate.Month ==_month)
                         .Count(),

                     })
                    .FirstOrDefaultAsync();

                

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("create"), Authorize]
        public async Task<ActionResult> CreateTransaction([FromBody] CreateTransactionRequest requestBody)
        {
           var user = await dbContext.Users.FindAsync(requestBody.UserId);
           var transactionType = await dbContext.TransactionTypes.FindAsync(requestBody.TransactionTypeId);

            if(transactionType == null || user == null) {
                return BadRequest("Invalid user or transaction type.");
            }

            var transaction = new Models.Domains.Transaction
            {
                UserId = requestBody.UserId,
                TransactionTypeId = requestBody.TransactionTypeId,
                User = user,
                TransactionType = transactionType,
                CreatedDate = requestBody.CreatedDate,
                UpdatedDate = DateTime.Now,
                Name = requestBody.Name,
                Description = requestBody.Description,
                Amount = requestBody.Amount,
                Status = 1,
            };

            dbContext.Transactions.Add(transaction);

            await dbContext.SaveChangesAsync();

            return Ok("Transaction created succesffuly");

        }

        [HttpPut]
        [Route("update/{Id:guid}"), Authorize]
        public async Task<ActionResult> UpdateTransaction([FromRoute] Guid Id, UpdateTransactionRequest requestBody)
        {
            try
            {
                var transaction = await dbContext.Transactions.FindAsync(Id);

                if(transaction == null)
                {
                    return NotFound("Transaction not found.");
                }

                if(requestBody.TransactionTypeId != null)
                {
                    var transactionType = await dbContext.TransactionTypes.FindAsync(requestBody.TransactionTypeId);

                    if (transactionType == null)
                    {
                        return BadRequest("Invalid transaction type");
                    }

                    transaction.TransactionType = transactionType;
                    transaction.TransactionTypeId = (Guid)requestBody.TransactionTypeId;

                }

                transaction.Description = requestBody.Description;
                transaction.CreatedDate = requestBody.CreatedDate;
                transaction.UpdatedDate = DateTime.Now;
                transaction.Amount = requestBody.Amount;
                
                
                await dbContext.SaveChangesAsync();
                return Ok("Transaction updated");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("remove/{Id:guid}"),Authorize]
        public async Task<ActionResult> RemoveTransaction([FromRoute] Guid Id)
        {
            try
            {
                var transaction = await dbContext.Transactions.FindAsync(Id);

                if (transaction == null)
                {
                    return NotFound("Transaction not found.");
                }

                transaction.Status = 2;
                transaction.UpdatedDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return Ok("Transaction removed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
