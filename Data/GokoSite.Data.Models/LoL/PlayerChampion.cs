namespace GokoSite.Data.Models.LoL
{
    public class PlayerChampion
    {
        public int PlayerId { get; set; }

        public virtual Player Player { get; set; }

        public int ChampionId { get; set; }

        public virtual Champion Champion { get; set; }

    }
}
