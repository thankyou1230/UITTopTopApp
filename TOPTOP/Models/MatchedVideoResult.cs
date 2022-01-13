using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class MatchedVideoResult
    {
        public MatchedVideoResult(Video video, User user)
        {
            this.Video = video;
            this.OwnerProfile = user;
        }
        public Video Video { get; set; }
        public User OwnerProfile { get; set; }
    }
}
