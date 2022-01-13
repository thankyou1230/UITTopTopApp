using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class User
    {
        public User(string email, string avatar, string nickName, string bio, DateTime lastLogin, int isLocked)
        {
            this.Email = email;
            this.Avatar = avatar;
            this.NickName = nickName;
            this.Bio = bio;
            this.LastLogin = lastLogin;
            this.IsLocked = isLocked;
        }
        public User()
        {

        }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string NickName { get; set; }
        public string Bio { get; set; }
        public DateTime LastLogin { get; set; }
        public int IsLocked { get; set; }
    }
}
