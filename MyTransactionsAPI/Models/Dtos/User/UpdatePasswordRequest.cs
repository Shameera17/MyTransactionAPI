namespace MyTransactionsAPI.Models.Dtos.User
{
    public class UpdatePasswordRequest
    {
        public Guid? Id { get; set; }
        public required string Email { get; set; }
        public required string OldPassword { get; set; } 
        public required string NewPassword { get; set; }

    }
}
