namespace GokoSite.Web.ViewModels.Games.DTOs
{
    using System.Collections.Generic;
    using System.Linq;

    public class TeamDTO
    {
        public List<PlayerDTO> Players { get; set; }

        public string State { get; set; }

        public int TotalKills => this.Players.Sum(p => int.Parse(p.KDA.Split(new char[] { '/' })[0]));

        public long MaxDmg => this.Players.Max(p => p.Damage);

        public long TotalGold { get; set; }

        public int DragonsSlain { get; set; }

        public int BaronsSlain { get; set; }

        public int TurretsDestroyed { get; set; }
    }
}
