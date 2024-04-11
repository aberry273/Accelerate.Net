using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;

namespace Accelerate.Features.Account.Services
{
    public interface IAccountViewService
    {
        ManagePage GetManagePage(AccountUser user);
        AccountFormPage GetLoginPage(string? username);
        AccountFormPage GetRegisterPage(string? username, string? email);
        AccountFormPage GetForgotPasswordPage(string? usernameOrEmail);
    }
}
