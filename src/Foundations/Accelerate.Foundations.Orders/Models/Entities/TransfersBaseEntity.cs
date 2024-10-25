using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Orders.Models.Entities
{
    public class OrdersBaseEntity : BaseEntity
    {
        public OrdersStatusEnum Status { get; set; }
        // Address
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public int Postcode { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
    }
}
