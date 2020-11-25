namespace GokoSite.Web.ViewModels.Games.DTOs
{
    public class PlayerDTO
    {
        public string Username { get; set; }

        public string ProfileIconUrl { get; set; }

        public string KDA { get; set; }

        public long Damage { get; set; }

        public string CS { get; set; }

        public int Level { get; set; }

        public string FirstSumSpellUrl { get; set; }

        public string SecondSumSpellUrl { get; set; }

        public ChampionDTO Champion { get; set; }
    }
}
