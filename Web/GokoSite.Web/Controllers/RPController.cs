namespace Suls.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using GokoSite.Services.Data;
    using GokoSite.Web.ViewModels.Forums;
    using Microsoft.AspNetCore.Mvc;

    public class RPController : Controller
    {
        private readonly IRPServerService rPServerService;
        private readonly IForumsService forumsService;

        public RPController(IRPServerService rPServerService, IForumsService forumsService)
        {
            this.rPServerService = rPServerService;
            this.forumsService = forumsService;
        }

        public IActionResult Home()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        public IActionResult Rules()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        public IActionResult Players()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var viewModel = this.rPServerService.GetPlayers();

            return this.View(viewModel);
        }


        public IActionResult WhitelistApps()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        public IActionResult PoliceWhitelist()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        public IActionResult MedicWhitelist()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        public IActionResult ServerWhitelist()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        // Forum Routes
        public async Task<IActionResult> Forum()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = await this.forumsService.GetAll(userId);

            return this.View(viewModel);
        }

        public IActionResult SingleForum(string postId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var viewModel = this.forumsService.GetPost(postId);

            return this.View(viewModel);
        }

        public IActionResult MyForums()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = this.forumsService.GetPersonalPosts(userId);

            return this.View(viewModel);
        }

        public IActionResult AddForum()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddForum(AddForumModel input)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string forumId = await this.forumsService.AddPost(input);
            await this.forumsService.AddPostToUser(userId, forumId);

            return this.Redirect("/RP/Forum");
        }

        public async Task<IActionResult> Like(string id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.forumsService.Like(id, userId);

            return this.Redirect("/RP/Forum");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            await this.forumsService.DeletePost(id);

            return this.Redirect("/RP/MyForums");
        }

        public IActionResult Edit(string id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var viewModel = this.forumsService.GetPost(id);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(EditForumInputModel input)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            await this.forumsService.EditPost(input);

            return this.Redirect("/RP/MyForums");
        }
    }
}
