using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Media.Models.Data
{
    public class MediaBlobUploadRequest
    {
        public MediaBlobUploadRequest(IFormFile file)
        {
            File = file;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public IFormFile File { get; set; }
    }
}
