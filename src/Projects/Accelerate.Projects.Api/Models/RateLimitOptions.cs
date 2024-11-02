namespace Accelerate.Projects.Api.Models
{
    public class RateLimitOptions
    {
        public int TokenLimit { get; set; }
        public int QueueLimit { get; set; }
        public int ReplenishmentPeriod { get; set; }
        public int TokensPerPeriod { get; set; }
        public bool AutoReplenishment { get; set; }

    }
}
