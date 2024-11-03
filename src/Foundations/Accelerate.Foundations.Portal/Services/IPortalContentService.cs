using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Portal.Services
{
    public interface IPortalContentService
    {
        Task<BasePage> CreateAuthenticatedContent(UsersUser user);

    }
}
