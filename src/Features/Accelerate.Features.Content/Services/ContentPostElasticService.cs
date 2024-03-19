using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Microsoft.Extensions.Options;

namespace Accelerate.Features.Content.Services
{
    public class ContentPostElasticService :  IContentPostElasticService
    {
        public IElasticService _service;
        private string _index = "contentpost_index";
        
        public ContentPostElasticService(IElasticService service)
        {
            _service = service;
        }  
        public async Task<ContentPost> Index(ContentPostEntity doc)
        {
            await _service.CreateIndex(_index);
            var response = await _service.IndexDocument<ContentPostEntity>(doc, _index);
            return null;
            /*
            // Create if not exist
            await this.CreateIndex(_index);
            var doc = entity as ContentPost;
            doc.User = "Tester";
            var response = await this.IndexDocument(doc, _index);
            if (response.IsValidResponse)
            {
                return doc;
            }
            return null;
            */
        }
    }
}
