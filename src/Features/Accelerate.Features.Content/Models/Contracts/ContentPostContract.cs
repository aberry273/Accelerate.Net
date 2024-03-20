using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Models.Contracts
{
    public class ContentPostContract
    {
        public ContentPostEntity Entity => _entity;
        ContentPostEntity _entity;
        public ContentPostContract(ContentPostEntity entity)
        {
            _entity = entity;
        }
    }
}
