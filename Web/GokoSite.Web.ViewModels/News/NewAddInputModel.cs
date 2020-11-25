namespace GokoSite.Web.ViewModels.News
{
    using System.ComponentModel.DataAnnotations;

    public class NewAddInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Image { get; set; }
    }
}
