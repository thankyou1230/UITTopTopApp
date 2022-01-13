using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class MatchedUserResult
    {
        public MatchedUserResult(User user, int count)
        {
            this.Profile = user;
            this.FollowerCount = count;
        }
        public User Profile { get; set; }
        public int FollowerCount { get; set; }
    }
}
