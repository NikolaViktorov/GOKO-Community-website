namespace GokoSite.Web.ViewModels.News
{
    using System;

    public class NewDetailsPageViewModel
    {
        public string NewId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public string AuthorUsername { get; set; }

        public DateTime UploadedOn { get; set; }
    }
}
