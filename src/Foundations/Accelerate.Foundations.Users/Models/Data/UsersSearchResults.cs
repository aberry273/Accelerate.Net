using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Users.Models.Data
{
    public class UsersSearchResults
    {
        public List<UsersUserDocument> Users { get; set; } = new List<UsersUserDocument>();
    }
}
