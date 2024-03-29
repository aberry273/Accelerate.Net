using Accelerate.Foundations.Account.Models.Entities;

namespace Accelerate.Foundations.Account.Models
{
    public class AccountUserDocument
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public string Image { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
