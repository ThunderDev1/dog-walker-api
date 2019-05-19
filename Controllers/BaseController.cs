
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string UserId
        {
            get
            {
                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                string userId = user.FindFirst("sub").Value;
                return userId;
            }
        }
    }
}