using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Fireblocks.Models.Data
{
    public class FireblocksExternalWallet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CustomerRefId { get; set; }
        public List<FireblocksWalletAsset> Assets { get; set; }
    }
}
