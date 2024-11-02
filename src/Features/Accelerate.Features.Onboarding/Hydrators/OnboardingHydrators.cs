using Accelerate.Features.Onboarding.Models.Data;
using Accelerate.Foundations.Users.Models;

namespace Accelerate.Features.Content.Hydrators
{
    public static class OnboardingHydrators
    {
        public static void Hydrate(this SignUpFormDataConsumer formData, SignUpFormSessionData data)
        {
            data.Firstname = formData.Firstname;
            data.Lastname = formData.Lastname;
            data.Country = formData.Country;
            data.Email = formData.Email;
        }
        public static void Hydrate(this SignUpFormDataBusiness formData, SignUpFormSessionData data)
        {
            data.Firstname = formData.Firstname;
            data.Lastname = formData.Lastname;
            data.Country = formData.Country;
            data.Email = formData.Email;
            data.Industry = formData.Industry;
            data.Website = formData.Website;
            data.Volume = formData.Volume;
            data.CompanyName = formData.CompanyName;
        }
    }
}
