namespace Accelerate.Features.Onboarding.Models.Data
{
    public class FinalizeFormRequestBusiness : SignUpFormAccountRequest
    {
        public string CompanyName { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string TaxId { get; set; }
        public string AccountType { get; set; }
        public Guid UserId { get; set; }
    }
}
