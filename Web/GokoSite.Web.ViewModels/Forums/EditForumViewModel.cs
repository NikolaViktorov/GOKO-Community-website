namespace GokoSite.Web.ViewModels.Forums
{
    using System.ComponentModel.DataAnnotations;

    public class EditForumViewModel
    {
        public string ForumId { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public string Text { get; set; }

        public int Likes { get; set; }
    }
}
