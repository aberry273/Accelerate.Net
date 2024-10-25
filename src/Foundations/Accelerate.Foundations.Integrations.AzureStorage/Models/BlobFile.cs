using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AzureStorage.Models
{
    public struct BlobFile
    {
        public Guid id { get; set; }
        public byte[] fileData { get; set; }
        public string fileMimeType { get; set; }
        public string fileName { get; set; }
        public string blobPath { get; set; }
    }
}
