using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;

namespace Accelerate.Features.Content.Hydrators
{
    public static class ContentHydrators
    {
        public static void Hydrate(this UsersUserDocument user, ContentPostUserProfileSubdocument document)
        {
            document.Id = user.Id;
            document.Date = user.CreatedOn.ToString();
            document.Username = user.Username;
            document.DisplayName = $"{user.Firstname} {user.Lastname}";
            document.Img = $"{user.Image}?w=50";
            document.Icon = "calendar";
        }
    }
}
