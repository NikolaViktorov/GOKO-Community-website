namespace Suls.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using GokoSite.Services.Data;
    using GokoSite.Web.ViewModels.Games;
    using Microsoft.AspNetCore.Mvc;

    public class LOLController : Controller
    {
        private readonly IGamesService gamesService;
        private readonly INewsService newsService;
        private readonly IRegionsService regionsService;

        public LOLController(
            IGamesService gamesService,
            INewsService newsService,
            IRegionsService regionsService)
        {
            this.gamesService = gamesService;
            this.newsService = newsService;
            this.regionsService = regionsService;
        }

        public IActionResult Home()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var viewModel = this.newsService.GetNews();

            return this.View(viewModel);
        }

        public async Task<IActionResult> lolapp()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var regions = await this.regionsService.GetRegions();
            this.ViewBag.Regions = regions;

            return this.View();
        }

        public async Task<IActionResult> GetGames(GetGamesInputModel input)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            IEnumerable<HomePageGameViewModel> viewModel = new List<HomePageGameViewModel>();
            try
            {
                var games = await this.gamesService.GetGamesAsync(input);
                viewModel = await this.gamesService.GetModelByMatches(games, input.RegionId);
            }
            catch (System.Exception e)
            {
                if (e.Message == "404, Resource not found")
                {
                    this.ModelState.AddModelError("lol", "There are no results recorded for the given Username.");
                }
                else
                {
                    this.ModelState.AddModelError("lol", e.Message);
                }
            }

            if (!this.ModelState.IsValid)
            {
                var regions = await this.regionsService.GetRegions();
                this.ViewBag.Regions = regions;
                return this.View("lolapp");
            }

            return this.View("games", viewModel);
        }

        public IActionResult Collection()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = this.gamesService.GetCollectionGames(userId);

            return this.View(viewModel);
        }

        // add
        public async Task<IActionResult> AddToCollection(long gameId, int regionId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userGameCount = this.gamesService.GetGameCount(userId);

            if (userGameCount < 10)
            {
                await this.gamesService.AddGameToCollection(gameId, regionId);
                this.gamesService.AddGameToUser(userId);
            }

            return this.Redirect("/LOL/Collection");
        }

        public async Task<IActionResult> CollectionDetails(long gameId, int regionId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            HomePageGameViewModel viewModel = new HomePageGameViewModel();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                 viewModel = await this.gamesService.GetModelByGameId(gameId, regionId, userId);
            }
            catch (System.Exception e)
            {
                 this.ModelState.AddModelError("details", e.Message);
            }

            if (!this.ModelState.IsValid)
            {
                return this.Redirect("DetailsError");
            }

            return this.View(viewModel);
        }

        public IActionResult Remove(long gameId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/Identity/Account/Login");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            this.gamesService.RemoveGameFromCollection(userId, gameId);

            return this.Redirect("/LOL/Collection");
        }

        public IActionResult New(string newId)
        {
            var viewModel = this.newsService.GetNew(newId);

            return this.View(viewModel);
        }

        public IActionResult DetailsError()
        {
            return this.View();
        }
    }
}
