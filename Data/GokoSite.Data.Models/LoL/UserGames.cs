using System;
using System.Collections.Generic;
using System.Text;

namespace GokoSite.Data.Models.LoL
{
    public class UserGames
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int GameId { get; set; }

        public virtual Game Game { get; set; }
    }
}
