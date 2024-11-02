namespace Accelerate.Features.Onboarding.Models.Data
{
    public class SignUpAuthenticateOtpData
    {
        public string Provider { get; set; }
        public string Code { get; set; }
        public Guid UserId { get; set; }
        public string Id { get; set; }

    }
}
