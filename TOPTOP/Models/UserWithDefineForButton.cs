using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    class UserWithDefineForButton
    {
        public UserWithDefineForButton(User user, string currentUser, List<User> currentUserFollowings)
        {
            this.User = user;
            if(currentUser==user.Email)
            {
                this.ButtonColor = "Gray";
                this.ButtonText = "Chính mình";
                this.IsEnable = false;
            }
            else if(currentUserFollowings.Contains(user))
            {
                this.ButtonColor = "#FF66C4";
                this.ButtonText = "Bỏ theo dõi";
                this.IsEnable = true;
            }
            else
            {
                this.ButtonColor = "#17D8E2";
                this.ButtonText = "Theo dõi";
                this.IsEnable = true;
            }
        }
        public User User { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonText { get; set; }
        public bool IsEnable { get; set; }
    }
}
