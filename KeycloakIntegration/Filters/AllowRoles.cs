using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace KeycloakIntegration.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowRoles : Attribute, IActionFilter
    {
        private readonly string[] _requiredRoles;

        public AllowRoles(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            // If user has no session or we don't have its data return unauthorized.
            if (
                context.HttpContext.User == null
                || context.HttpContext.User.Identity == null
                || !context.HttpContext.User.Identity.IsAuthenticated
            )
            {
                context.Result = new UnauthorizedObjectResult(null);
                return;
            }

            // Get the user resource_access claim to check if it has an allowed role
            var userClaims = context.HttpContext.User.Claims;
            var resourceAccessClaim = userClaims.FirstOrDefault(c => c.Type == "resource_access");

            if (resourceAccessClaim == null)
            {
                context.Result = new UnauthorizedObjectResult(null);
                return;
            }

            var resourceAccessJson = JObject.Parse(resourceAccessClaim.Value);
            bool isAuthorized = _requiredRoles.Any(role =>
            {
                var parts = role.Split('@');
                if (parts.Length != 2) return false; // Invalid format
                var client = parts[0];
                var roleNeeded = parts[1];

                var clientRoles = (JArray?)resourceAccessJson[client]?["roles"];
                return clientRoles != null && clientRoles.Any(r => r.ToString() == roleNeeded);
            });

            if (!isAuthorized)
            {
                context.Result = new UnauthorizedObjectResult(null);
            }

        }
    }


}
