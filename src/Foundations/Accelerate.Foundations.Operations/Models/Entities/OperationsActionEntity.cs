using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Operations.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Models.Entities
{
    public enum OperationsActionState
    {
        Hidden, Public, Archived
    }
    [Table("OperationsActions")]
    public class OperationsActionEntity : BaseOperationsEntity
    {
        #region Required 
        #endregion
        public OperationsActionState State { get; set; }
        public Guid? SystemId { get; set; }
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public string Settings { get; set; }
        public string Data { get; set; }
        public string Action { get; set; }
        public ICollection<OperationsActivityEntity>? Activities { get; set; }
    }
}
