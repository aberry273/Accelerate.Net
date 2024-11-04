using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    public class RatesExchangeEntity : BaseEntity
    {
        public required string FromCurrency { get; set; }
        public required string ToCurrency { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public required decimal Rate { get; set; }
        public required DateTime LastRefreshed { get; set; }
        public required string TimeZone { get; set; }
    }
}
