namespace GokoSite.Web.ViewModels.Games
{
    using System.Collections.Generic;
    using System.Linq;

    using GokoSite.Web.ViewModels.Games.DTOs;

    public class HomePageGameViewModel
    {
        public long GameId { get; set; }

        public int RegionId { get; set; }

        public TeamDTO BlueTeam { get; set; }

        public TeamDTO RedTeam { get; set; }

        public long MaxDmg => new List<TeamDTO>() { BlueTeam, RedTeam }.Max(t => t.MaxDmg);

    }
}
