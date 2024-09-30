using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Models.Entities
{
    public enum OperationsJobState
    {
        Hidden, Published, Archived
    }
    [Table("OperationsJobs")]
    public class OperationsJobEntity : BaseOperationsEntity
    {
        #region Required 
        #endregion
        public OperationsJobState State { get; set; }
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public string Schedule { get; set; }
        [Required]
        public Guid ActionId { get; set; }
    }
}
