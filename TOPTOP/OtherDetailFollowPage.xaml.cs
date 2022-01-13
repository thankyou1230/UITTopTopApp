using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.MainTabs;
using TOPTOP.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherDetailFollowPage : ContentPage
    {
        public string currentUser;
        public string listOwner;
        public List<User> currentUserFollowings;
        List<UserWithDefineForButton> followings;
        List<UserWithDefineForButton> followers;
        public OtherDetailFollowPage()
        {
            InitializeComponent();
        }
        public void CreateDataSource(List<User> inputFollowings, List<User> inputFollowers)
        {
            this.followings = new List<UserWithDefineForButton>();
            this.followers = new List<UserWithDefineForButton>();
            foreach (User user in inputFollowings)
            {
                this.followings.Add(new UserWithDefineForButton(user, this.currentUser, this.currentUserFollowings));
            }
            foreach (User user in inputFollowers)
            {
                this.followers.Add(new UserWithDefineForButton(user, this.currentUser, this.currentUserFollowings));
            }
        }
        public OtherDetailFollowPage(List<User> Followings, List<User> Followers, string currentUser, User listOwner, List<User> currentUserFollowings)
        {
            InitializeComponent();
            Title.Text = "Theo dõi của " + listOwner.NickName;
            this.currentUserFollowings = currentUserFollowings;
            this.currentUser = currentUser;
            this.listOwner = listOwner.Email;
            CreateDataSource(Followings, Followers);
            FollowingView.ItemsSource = this.followings;
            FollowerView.ItemsSource = this.followers;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            HttpClient httpClient = new HttpClient();
            if (btn.Text == "Theo dõi")
            {
                btn.Text = "Đang xử lí";
                //Logic theo dõi
                try
                {
                    string followTo = btn.ClassId;
                    var content = new Dictionary<string, string> { { "FollowFrom", currentUser }, { "FollowTo", followTo } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/FollowRequest", body);
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        btn.Text = "Hủy theo dõi";
                        btn.BackgroundColor = Color.FromHex("#FF66C4");
                        btn.IsEnabled = true;
                    }
                    else
                    {
                        btn.Text = "Theo dõi";
                        btn.BackgroundColor = Color.FromHex("#17D8E2");
                        btn.IsEnabled = true;
                    }
                }
                catch (Exception)
                {
                    btn.Text = "Theo dõi";
                    btn.BackgroundColor = Color.FromHex("#17D8E2");
                    btn.IsEnabled = true;
                }
            }
            else //unfnollow
            {
                btn.Text = "Đang xử lí";
                try
                {
                    string unfollowTo = btn.ClassId;
                    var content = new Dictionary<string, string> { { "UnfollowFrom", currentUser }, { "UnfollowTo", unfollowTo } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/UnfollowRequest", body);
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        btn.Text = "Theo dõi";
                        btn.BackgroundColor = Color.FromHex("#17D8E2");
                        btn.IsEnabled = true;
                    }
                    else
                    {
                        btn.Text = "Hủy theo dõi";
                        btn.BackgroundColor = Color.FromHex("#FF66C4");
                        btn.IsEnabled = true;
                    }
                }
                catch (Exception)
                {
                    btn.Text = "Hủy theo dõi";
                    btn.BackgroundColor = Color.FromHex("#FF66C4");
                    btn.IsEnabled = true;
                }
            }
        }
        private void FollowingView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FollowingView.SelectedItem != null)
            {
                UserWithDefineForButton selectedUser = (UserWithDefineForButton)FollowingView.SelectedItem;
                if (selectedUser.User.Email==this.currentUser)
                {
                    Navigation.PushAsync(new ProfileTab(this.currentUser));
                }    
                else
                {
                    Navigation.PushAsync(new OtherProfilePage(this.currentUser, selectedUser.User));
                }    
                FollowingView.SelectedItem = null;
            }
        }
        private void FollowerView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FollowerView.SelectedItem != null)
            {
                UserWithDefineForButton selectedUser = (UserWithDefineForButton)FollowerView.SelectedItem;
                if (selectedUser.User.Email == this.currentUser)
                {
                    Navigation.PushAsync(new ProfileTab(this.currentUser));
                }
                else
                {
                    Navigation.PushAsync(new OtherProfilePage(this.currentUser, selectedUser.User));
                }
                FollowerView.SelectedItem = null;
            }
        }
    }
}