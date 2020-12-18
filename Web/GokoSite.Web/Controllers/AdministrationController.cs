namespace GokoSite.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using GokoSite.Common;
    using GokoSite.Services.Data;
    using GokoSite.Web.ViewModels.Administration;
    using GokoSite.Web.ViewModels.News;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class AdministrationController : Controller
    {
        private readonly IAuthorizationsService authorizationsService;
        private readonly INewsService newsService;
        private readonly IForumsService forumsService;

        public AdministrationController(
            IAuthorizationsService authorizationsService,
            INewsService newsService,
            IForumsService forumsService)
        {
            this.authorizationsService = authorizationsService;
            this.newsService = newsService;
            this.forumsService = forumsService;
        }
        
        public IActionResult AdminPanel()
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        public IActionResult AddAdmin()
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddAdmin(AddAdminInputModel input)
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            await this.authorizationsService.AddAdministrator(input.Email);

            return this.Redirect("/");
        }

        public IActionResult AddNew()
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddNew(NewAddInputModel input)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            await this.newsService.AddNew(input, userId);

            return this.Redirect("/LOL/Home");
        }

        public async Task<IActionResult> RemoveNew(string id)
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            var isDeleted = await this.newsService.RemoveNew(id);
            return this.Redirect("/LOL/Home");
        }

        public async Task<IActionResult> RemovePost(string id)
        {
            if (!this.User.IsInRole("Administrator"))
            {
                return this.Redirect("/");
            }

            await this.forumsService.DeletePost(id);
            return this.Redirect("/RP/Forum");
        }
    }
}
