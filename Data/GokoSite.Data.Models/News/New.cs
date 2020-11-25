namespace GokoSite.Data.Models.News
{
    using System;

    public class New
    {
        public New()
        {
            this.NewId = Guid.NewGuid().ToString();
        }

        public string NewId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public DateTime UploadedOn { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
