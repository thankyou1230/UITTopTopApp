using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using TOPTOP.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherProfilePage : ContentPage
    {
        public Profile profile;
        public string other;
        public string currentUser;
        public OtherProfilePage()
        {
            InitializeComponent();
        }
        public bool FetchProfile(string email)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "email", email } };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetUserProfile", body);
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                this.profile = JsonConvert.DeserializeObject<Profile>(responseContent.Result);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void InitProfile(string userEmail)
        {
            if (FetchProfile(userEmail))
            {
                Title.Text = "Hồ sơ của " + this.profile.MainProfile.NickName;
                avatarBtn.Source = this.profile.MainProfile.Avatar;
                nicknameLabel.Text = "@" + this.profile.MainProfile.NickName;
                followerCount.Text = this.profile.Followers.Count().ToString();
                followingCount.Text = this.profile.Followings.Count().ToString();
                likeCount.Text = this.profile.LikeCount.ToString();
                if (this.profile.MainProfile.Bio != "")
                {
                    bioLabel.IsVisible = true;
                    bioLabel.Text = this.profile.MainProfile.Bio;
                }
                else
                {
                    bioLabel.IsVisible = false;
                }
                OwnVideoView.ItemsSource = this.profile.Videos.Where(video => video.IsPrivate == 0);
                LikedVideoView.ItemsSource = this.profile.LikedVideos.Where(video => video.IsPrivate == 0);
                if (this.profile.Followers.Where(follower=>follower.Email==this.currentUser).Count()==1)
                {
                    followBtn.BackgroundColor = Color.FromHex("#FF66C4");
                    followBtn.Text = "Hủy theo dõi";
                }
                else
                {
                    followBtn.BackgroundColor = Color.FromHex("#17D8E2");
                    followBtn.Text = "Theo dõi";
                }    
            }
            else
            {
                DisplayAlert("Thông báo", "Không thể kết nối tới máy chủ", "OK");
            }

        }
        public OtherProfilePage(string currentUser, User otherMainProfile)
        {
            InitializeComponent();
            Title.Text = "Hồ sơ của " + otherMainProfile.NickName;
            this.currentUser = currentUser;
            this.other = otherMainProfile.Email;
            //InitProfile(this.other);
        }
        public OtherProfilePage(string currentUser, string other)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.other = other;
            //InitProfile(this.other);
        }
        protected override void OnAppearing()
        {
            InitProfile(this.other);
            base.OnAppearing();
        }

        private void avatarBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new AvatarPopUp(this.profile.MainProfile.Avatar));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //follow detail tap
            try
            {
                HttpClient httpClient = new HttpClient();
                //refetch infomation of current user
                var content = new Dictionary<string, string> { { "email", this.currentUser } };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetUserProfile", body);
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                Profile p = JsonConvert.DeserializeObject<Profile>(responseContent.Result);
                Navigation.PushAsync(new OtherDetailFollowPage(this.profile.Followings, this.profile.Followers, this.currentUser, this.profile.MainProfile, p.Followings));
                //return true;
            }
            catch (Exception)
            {
                DisplayAlert("Lỗi", "Không thể kết nối tới server", "OK");
            }

        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            //like tap
        }

        private void followBtn_Clicked(object sender, EventArgs e)
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
                    var content = new Dictionary<string, string> { { "FollowFrom", currentUser }, { "FollowTo", this.other } };
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
                    var content = new Dictionary<string, string> { { "UnfollowFrom", currentUser }, { "UnfollowTo", this.other } };
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
    }
}