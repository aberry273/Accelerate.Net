using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;

namespace Accelerate.Foundations.Media.Hydrators
{
    public static class MediaHydrators
    {
        public static void Hydrate(this MediaBlobEntity entity, MediaBlobDocument document, UserSubdocument userProfile = null)
        {
            document.Status = entity.Status;
            document.UserId = entity.UserId;
            document.Name = entity.Name;
            document.FilePath = entity.FilePath;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.Tags = entity.TagItems;
            document.Id = entity.Id;
            document.Type = Enum.GetName(entity.Type);
            document.Tags = entity.TagItems;
            document.User = userProfile ?? 
                new UserSubdocument()
                {
                    Username = "Anonymous"
                };
        }
    }
}
