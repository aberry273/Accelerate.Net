using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Account.Services
{
    public interface IAccountViewService
    {
        ManagePage GetManagePage(AccountUser user);
        AccountFormPage GetLoginPage(string? username);
        AccountFormPage GetRegisterPage(string? username, string? email);
        AccountFormPage GetForgotPasswordPage(string? usernameOrEmail);
        AccountFormPage GetConfirmAccountPage(string? userId);
        AccountFormPage GetResetPasswordPage(string? userId, string? code);
        AccountFormPage GetForgotPasswordConfirmationPage();
        public List<NavigationFilter> CreatePostSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        public List<NavigationFilter> CreateMediaSearchFilters(SearchResponse<MediaBlobDocument> aggregateResponse);
    }
}
