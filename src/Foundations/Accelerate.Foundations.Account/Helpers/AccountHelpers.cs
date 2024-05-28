using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Models;

namespace Accelerate.Foundations.Account.Helpers
{
    public class AccountHelpers
    {
        public static UserProfile CreateUserProfile(AccountUser user)
        {
            return user != null ? new UserProfile()
            {
                IsAuthenticated = true,
                Username = user.UserName,
                UserId = user?.Id,
                Image = user?.AccountProfile?.Image
            } : new UserProfile()
            {
                IsAuthenticated = false,
                Username = "Anonymous",
                UserId = null,
                Image = null
            };
        }
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new System.Text.StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return $"0{res}";
        }
    }
}
