
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accelerate.Foundations.Common.Services;

namespace Accelerate.Foundations.Integrations.Kindentpay.Services
{
    public class KendentpayService : IKendentpayService
    {
        IResilientHttpClient _resilientHttpClient;
        public KendentpayService(IResilientHttpClient resilientHttpClient)
        {

        }
    }
}
