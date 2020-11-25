namespace GokoSite.Data.Models.LoL
{
    using System.Collections.Generic;

    public class Player
    {
        public Player()
        {
            this.PlayerChampions = new HashSet<PlayerChampion>();
        }

        public int PlayerId { get; set; }

        public string Username { get; set; }

        public string ProfileIconUrl { get; set; }

        public int TeamId { get; set; }

        public virtual ICollection<PlayerChampion> PlayerChampions { get; set; }
    }
}
