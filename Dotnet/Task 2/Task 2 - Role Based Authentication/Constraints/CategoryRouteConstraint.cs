using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace FoodOrderingSystemAPI.Constraints
{
    public class CategoryRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var routeValue) && routeValue is string value)
            {
                return value.All(char.IsLetter);
            }
            return false;
        }
    }
}
