using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class VideoWithOwnerDetails
    {
        public Video VideoDetails { get; set; }
        public User OwnerDetails { get; set; }
    }
}
