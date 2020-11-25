namespace GokoSite.Data.Models.LoL
{
    using System.Collections.Generic;

    public class Champion
    {
        public Champion()
        {
            this.PlayerChampions = new HashSet<PlayerChampion>();
        }

        public int ChampionId { get; set; }

        public string ChampionName { get; set; }

        public string ChampionIconUrl { get; set; }

        public string ChampionRiotId { get; set; }

        public virtual ICollection<PlayerChampion> PlayerChampions { get; set; }

    }
}
