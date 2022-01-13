using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class Video
    {
        public string Url { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime UploadDate { get; set; }
        public string ThumbnailUrl { get; set; }
        public int IsPrivate { get; set; }
        public string IsLiked { get; set; }
    }
}
