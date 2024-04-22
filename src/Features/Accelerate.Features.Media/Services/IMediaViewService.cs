using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Media.Services
{
    public interface IMediaViewService
    {
        List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters);
    }
}
