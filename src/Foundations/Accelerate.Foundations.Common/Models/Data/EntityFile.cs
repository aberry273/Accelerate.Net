using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Data
{
    public class EntityFile
    {
        public Guid? UserId { get; set; }
        public Guid? Id { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
