using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    public enum RatesOrderStatus
    {
        Open, Closed, Cancelled, Completed
    }
    [Table("RatesConversionOrderEntity")]
    public class RatesConversionOrderEntity : RatesConversionBaseEntity
    {
        #region Required 
        #endregion 
        public RatesOrderStatus Status { get; set; } 
        public string? Memo { get; set; }
        public string? UniqueId { get; set; }
        public string? TermsAgreement { get; set; }
    }
}
