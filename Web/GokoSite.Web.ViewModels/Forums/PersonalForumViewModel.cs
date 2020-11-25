using System;
using System.Collections.Generic;
using System.Text;

namespace GokoSite.Web.ViewModels.Forums
{
    public class PersonalForumViewModel
    {
        public string OwnerId { get; set; }

        public string ForumId { get; set; }

        public string ForumTopic { get; set; }

        public string ForumText { get; set; }
    }
}
