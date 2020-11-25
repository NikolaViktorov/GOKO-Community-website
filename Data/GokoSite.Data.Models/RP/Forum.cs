namespace GokoSite.Data.Models.RP
{
    using System;
    using System.Collections.Generic;

    public class Forum
    {
        public Forum()
        {
            this.ForumId = Guid.NewGuid().ToString();
            this.UserForums = new HashSet<UserForums>();
        }

        public string ForumId { get; set; }

        public string ForumTopic { get; set; }

        public string ForumText { get; set; }

        public int Likes { get; set; }

        public virtual ICollection<UserForums> UserForums { get; set; }

    }
}
