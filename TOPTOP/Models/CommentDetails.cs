using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class CommentDetails
    {
        public CommentDetails(Comment comment, User user)
        {
            this.Comment = comment;
            this.User = user;
        }
        public Comment Comment { get; set; }
        public User User { get; set; }
    }
}
