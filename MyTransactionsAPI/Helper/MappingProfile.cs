using AutoMapper;
using MyTransactionsAPI.Models.Domains;
using MyTransactionsAPI.Models.Dtos.Transaction;
using MyTransactionsAPI.Models.Dtos.TransactionType;
using MyTransactionsAPI.Models.Dtos.User;

namespace MyTransactionsAPI.Helper
{
    public class MappingProfile :Profile
    {
        public MappingProfile() {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionType, TransactionTypeDto>();
            CreateMap<User, UserViewRequest>();
        }
    }
}
