namespace GokoSite.Web.Controllers
{
    using GokoSite.Data.Models;
    using GokoSite.Services.Data.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUsersService usersService;

        public AccountController(SignInManager<ApplicationUser> signInManager, IUsersService usersService)
        {
            this.signInManager = signInManager;
            this.usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/Identity/Home/ExternalLogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

    }
}
