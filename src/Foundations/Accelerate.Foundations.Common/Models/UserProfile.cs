using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models
{
    public class UserProfile
    {
        public bool IsDeactivated { get; set; }
        public bool IsAuthenticated { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Username {  get; set; }
        public string Image { get; set; }
    }
}
