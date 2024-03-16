﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace Accelerate.Features.Account.Attributes
{
    public class RedirectAuthenticatedRouteAttribute : ResultFilterAttribute
    {
        public string url { get; set; }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(url))
            {
                context.HttpContext.Response.Redirect(url);
            }
            base.OnResultExecuting(context);
        }
    }
}
