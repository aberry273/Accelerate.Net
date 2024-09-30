using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Models.Entities
{
    [Table("OperationsActivities")]
    public class OperationsActivityEntity : BaseEntity
    { 
        #region Required
        #endregion
        public Guid OperationsActionId { get; set; }
        public Guid? OperationsJobId { get; set; }
        public virtual OperationsActionEntity OperationsAction { get; set; }
        public string Data { get; set; }
        public string Result { get; set; }
        public bool Success { get; set; }
        public Guid? UserId { get; set; }
    }
}
