namespace GokoSite.Web.ViewModels.Forums
{
    public class ForumViewModel
    {
        public string ForumId { get; set; }

        public string ForumTopic { get; set; }

        public string ForumText { get; set; }

        public int Likes { get; set; }

        public bool Liked { get; set; }
    }
}
