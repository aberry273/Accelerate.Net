using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Models;

namespace Accelerate.Foundations.Users.Helpers
{
    public class UsersHelpers
    {
        public static UserProfile CreateUserProfile(UsersUser user)
        {
            return user != null ? new UserProfile()
            {
                IsAuthenticated = true,
                IsDeactivated = user.Status == UsersUserStatus.Deactivated,
                Name = user.UsersProfile != null 
                    ? $"{user.UsersProfile.Firstname} {user.UsersProfile.Lastname}"
                    : user.UserName,
                Username = user.UserName,
                UserId = user?.Id,
                Domain = user.Domain,
                Image = user?.UsersProfile?.Image
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
