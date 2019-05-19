
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int UserId
        {
            get
            {
                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                string id = user.FindFirst("sub").Value;
                int userId = int.Parse(id);

                return userId;
            }
        }
    }
}