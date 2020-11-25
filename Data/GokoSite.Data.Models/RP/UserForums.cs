namespace GokoSite.Data.Models.RP
{
    public class UserForums
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ForumId { get; set; }

        public virtual Forum Forum { get; set; }
    }
}
