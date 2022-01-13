using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class Comment
    {
        public Comment(string user, string video, string CommentContent, DateTime commentTime, string isOwner)
        {
            this.User = user;
            this.Video = video;
            this.CommentContent = CommentContent;
            this.CommentTime = commentTime;
            this.IsOwner = isOwner;
        }

        public string User { get; set; }
        public string Video { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentTime { get; set; }
        public string IsOwner { get; set; }
    }
}
