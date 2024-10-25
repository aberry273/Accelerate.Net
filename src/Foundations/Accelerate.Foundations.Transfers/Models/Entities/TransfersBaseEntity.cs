using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transfers.Models.Entities
{
    public class TransfersBaseEntity : BaseEntity
    {
        public TransfersStatusEnum Status { get; set; }
        [Required]
        public required string Currency { get; set; }
        [Required]
        public required decimal Amount { get; set; }
        [Required]
        public required decimal Fee { get; set; }
        [Required]
        public required string OnBehalfOf { get; set; }
        public TransfersCustomerEntity? Customer { get; set; }
        [ForeignKey("TransfersCustomer")]
        public Guid? CustomerId { get; set; }
        public Guid? FundingSourceId { get; set; }
    }
}
