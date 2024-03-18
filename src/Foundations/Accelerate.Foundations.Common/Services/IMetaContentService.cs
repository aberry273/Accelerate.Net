using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IMetaContentService
    {
        BasePage CreatePageBaseContent(UserProfile? profile = null);
    }
}
