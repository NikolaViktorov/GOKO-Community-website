namespace GokoSite.Web.ViewModels.Players
{
    using System.Collections.Generic;

    public class HomePageViewModel
    {
        public List<string> PlayerNames { get; set; }

        public List<int> PlayerPings { get; set; }

        public int PlayersCount { get; set; }
    }
}
