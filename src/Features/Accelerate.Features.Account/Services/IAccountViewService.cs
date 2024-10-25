﻿using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Account.Services
{
    public interface IAccountViewService
    {
        ManagePage GetMentionsPage(AccountUser user);
        ManagePage GetNotificationsPage(AccountUser user, IEnumerable<ContentPostActivityEntity> activitiesResponse, int totalActivities);
        ManagePage GetManagePage(AccountUser user);
        Task<AccountFormPage> GetLoginPage(string? username);
        AccountFormPage GetRegisterPage(string? username, string? email);
        AccountFormPage GetForgotPasswordPage(string? usernameOrEmail);
        AccountFormPage GetConfirmAccountPage(string? userId);
        AccountFormPage GetResetPasswordPage(string? userId, string? code);
        AccountFormPage GetForgotPasswordConfirmationPage();
        NavigationFilter CreatePostNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        NavigationFilter CreateMediaNavigationFilters(SearchResponse<MediaBlobDocument> aggregateResponse);
        Task<AccountFormPage> GetExternalLoginExistingUser(string? username, string providerName);
        Task<AccountFormPage> GetExternalLoginNewUser(string? username);
        Task<AccountFormPage> GetExternalLoginDeactivatedUser(string? username);
        //public List<NavigationFilterItem> CreatePostSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        //public List<NavigationFilterItem> CreateMediaSearchFilters(SearchResponse<MediaBlobDocument> aggregateResponse);
    }
}
