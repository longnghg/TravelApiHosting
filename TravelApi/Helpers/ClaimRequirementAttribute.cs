using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace TravelApi.Helpers
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(CustomAuthorization))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
        public class CustomAuthorization : IAuthorizationFilter
        {
            readonly Claim _claim;

            public CustomAuthorization(Claim claim)
            {
                _claim = claim;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
                if (!hasClaim)
                {
                    context.Result = new ForbidResult($"You need {_claim.Value} role");
                }
            }
        }
    }
}
