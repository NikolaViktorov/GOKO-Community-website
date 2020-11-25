namespace GokoSite.Data.Models.LoL
{
    using System.Collections.Generic;

    public class Game
    {
        public Game()
        {
            this.Teams = new List<Team>();
            this.UserGames = new HashSet<UserGames>();
        }

        public int GameId { get; set; }

        public long RiotGameId { get; set; }

        public virtual List<Team> Teams { get; set; }

        public virtual ICollection<UserGames> UserGames { get; set; }

        public virtual Region Region { get; set; }

        public string RegionId { get; set; }
    }
}
