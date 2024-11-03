namespace Accelerate.Features.Onboarding.Models.Data
{
    public class FinalizeFormRequestIndividual : SignUpFormAccountRequest
    {
        public string DateOfBirth { get; set; }
        public string TaxId { get; set; }
        public Guid UserId { get; set; }
    }
}
