using Accelerate.Features.Profile.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Profile.Services
{
    public interface IProfileViewService
    {
        ManagePage GetMentionsPage(UsersUser user);
        ManagePage GetNotificationsPage(UsersUser user, IEnumerable<ContentPostActivityEntity> activitiesResponse, int totalActivities);
        ManagePage GetManagePage(UsersUser user);
        NavigationFilter CreatePostNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        NavigationFilter CreateMediaNavigationFilters(SearchResponse<MediaBlobDocument> aggregateResponse);
        
    }
}
