namespace GokoSite.Web.ViewModels.Forums
{
    using System.ComponentModel.DataAnnotations;

    public class AddForumModel
    {
        [Required]
        public string Topic { get; set; }

        [Required]
        [MaxLength(550)]
        public string Text { get; set; }
    }
}
