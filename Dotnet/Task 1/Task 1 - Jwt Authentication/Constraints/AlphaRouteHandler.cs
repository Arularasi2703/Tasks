using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Threading.Tasks;

namespace FoodOrderingSystemAPI.Constraints
{
    public class CategoryRouteHandler : IRouter
    {
        private readonly IRouter _defaultRouter;

        public CategoryRouteHandler(IRouter defaultRouter)
        {
            _defaultRouter = defaultRouter;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            var categoryName = context.RouteData.Values["categoryName"]?.ToString();
            if (!string.IsNullOrEmpty(categoryName) && !categoryName.All(char.IsLetter))
            {
                context.Handler = async ctx =>
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await ctx.Response.WriteAsync("Invalid category name. Only alphabetic characters are allowed.");
                };
                return;
            }

            await _defaultRouter.RouteAsync(context);
        }
    }
}
