namespace GokoSite.Web.ViewModels.Games
{
    using System.ComponentModel.DataAnnotations;

    public class GetGamesInputModel
    {
        [Required]
        [MaxLength(16)]
        public string Username { get; set; }

        [Required]
        [Range(1, 10)]
        public int Count { get; set; }

        [Required]
        public int RegionId { get; set; }
    }
}
