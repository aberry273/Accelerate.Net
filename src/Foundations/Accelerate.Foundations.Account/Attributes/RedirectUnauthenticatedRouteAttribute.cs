using Microsoft.AspNetCore.Mvc.Filters;

namespace Accelerate.Foundations.Account.Attributes
{
    public class RedirectUnauthenticatedRouteAttribute : ResultFilterAttribute
    {
        public string url { get; set; }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(url))
            {
                context.HttpContext.Response.Redirect(url);
            }
            base.OnResultExecuting(context);
        }
    }
}
