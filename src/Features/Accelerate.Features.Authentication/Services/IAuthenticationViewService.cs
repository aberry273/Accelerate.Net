using Accelerate.Features.Authentication.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Authentication.Services
{
    public interface IAuthenticationViewService
    {
        
        Task<AuthenticationFormPage> GetLoginPage(string? username);
        AuthenticationFormPage GetRegisterPage(string? username, string? email);
        AuthenticationFormPage GetForgotPasswordPage(string? usernameOrEmail);
        AuthenticationFormPage GetConfirmAccountPage(string? userId);
        AuthenticationFormPage GetResetPasswordPage(string? userId, string? code);
        AuthenticationFormPage GetForgotPasswordConfirmationPage(); 
        Task<AuthenticationFormPage> GetExternalLoginExistingUser(string? username, string providerName);
        Task<AuthenticationFormPage> GetExternalLoginNewUser(string? username);
        Task<AuthenticationFormPage> GetExternalLoginDeactivatedUser(string? username);
        //public List<NavigationFilterItem> CreatePostSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        //public List<NavigationFilterItem> CreateMediaSearchFilters(SearchResponse<MediaBlobDocument> aggregateResponse);
    }
}
