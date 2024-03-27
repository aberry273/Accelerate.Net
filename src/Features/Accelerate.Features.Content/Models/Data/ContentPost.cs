using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Models.Data
{
    public class ContentPost : ContentPostEntity
    {
        public string Username { get; set; }
        public int Agrees { get; set; }
        public int Disagrees { get; set; }
        public int Likes { get; set; }
    }
}
