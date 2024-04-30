using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Media.Models.Data
{
    public class MediaBlobUploadResult
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
