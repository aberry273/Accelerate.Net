using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Fireblocks.Models.Data
{
    public class FireblocksWalletAsset
    {
        public string Id { get; set; }
        public string Balance { get; set; }
        public string LockedAmount { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
        public string ActivationTime { get; set; }
    }
}
