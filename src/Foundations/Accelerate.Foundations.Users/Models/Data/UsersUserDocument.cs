using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;

namespace Accelerate.Foundations.Users.Models
{
    public class UsersUserDocument : EntityDocument
    {
        public string Username { get; set; }
        public string Domain { get; set; }
        public string Image { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public List<string> Roles { get; set; }
    }
}
