using HotelUI.Extentions;
using HotelUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelUI.Filters
{
    public class AuthFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICrudService _crudService;

        public AuthFilter(IHttpContextAccessor httpContextAccessor, ICrudService crudService)
        {
            _httpContextAccessor = httpContextAccessor;
            _crudService = crudService;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as ControllerBase;
            if (context.HttpContext.Request.Cookies["token"] == null)
            {
                context.Result = controller.RedirectToAction(
             actionName: "Login",
             controllerName: "Account",
             new { returnUrl = context.HttpContext.Request.Path }
         );
            }

            var PasswordResetRequired = _httpContextAccessor.HttpContext.Session.GetBool("PasswordResetRequired");
            if (PasswordResetRequired)
            {
                context.Result = new RedirectToActionResult("ResetPassword", "Account", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
