using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class Profile
    {
        public User MainProfile { get; set; }
        public List<User> Followers { get; set; }
        public List<User> Followings { get; set; }
        public int LikeCount { get; set; }
        public List<Video> Videos { get; set; }
        public List<Video> LikedVideos { get; set; }

    }
}
