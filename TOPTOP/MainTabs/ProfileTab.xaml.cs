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
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.MainTabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileTab : ContentPage
    {
        public string currenUser;
        public ProfileTab()
        {
            InitializeComponent();
        }
        public Profile profile;
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
            if(FetchProfile(userEmail))
            {
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
                OwnVideoView.ItemsSource = this.profile.Videos.Where(video=>video.IsPrivate==0);
                PrivateVideoView.ItemsSource = this.profile.Videos.Where(video => video.IsPrivate == 1);
                LikedVideoView.ItemsSource = this.profile.LikedVideos.Where(video => video.IsPrivate == 0);
            }
            else
            {
                this.DisplayToastAsync("Không thể kết nối tới máy chủ", 1000);
            }    

        }
        public ProfileTab(string currentUser)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            this.currenUser = currentUser;
            InitProfile(currentUser);
        }
        protected override void OnAppearing()
        {
            this.ReFreshView.IsRefreshing = true;
            base.OnAppearing();
        }
        public void ResetPage()
        {
            this.ReFreshView.IsRefreshing = true;
            base.OnAppearing();
        }

        private void avatarBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new AvatarPopUp(this.profile.MainProfile.Avatar));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //chuyển qua trang follow detail
            Navigation.PushAsync(new DetailFollowPage(this.profile.Followings,this.profile.Followers,this.profile.MainProfile.Email));
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Image a = (Image)sender;
            Navigation.PushPopupAsync(new SharePopup(a.ClassId));
        }

        private void editBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditProfilePage(this.profile.MainProfile));
        }

        private async void SignOutBtn_Clicked(object sender, EventArgs e)
        {
            //confirm logout
            var confirm = await DisplayAlert("", "Bạn có chắc chắn đăng xuất?", "Đăng xuất", "Hủy");
            if(confirm)
            {
                var db = new LocalDatabase();
                db.EmptyAllCurrentUsers();
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }    
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            InitProfile(this.currenUser);
            var a = (RefreshView)sender;
            a.IsRefreshing = false;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Image selected = (Image)sender;
            await Navigation.PushAsync(new VideoTab(this.profile.MainProfile, selected.ClassId));
        }

        private void ContentPage_Focused(object sender, FocusEventArgs e)
        {
            ReFreshView.IsRefreshing = true;
        }

        private void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            this.OnAppearing();
        }

        private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Chọn hành động", "Cancel", null, "Chuyển video sang chế độ riêng tư");
            if (action== "Chuyển video sang chế độ riêng tư")
            {
                var video = (Image)sender;
                try
                {
                    HttpClient httpClient = new HttpClient();
                    //fetch infomation
                    var content = new Dictionary<string, string> { { "url", video.ClassId } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/HideVideo", body);
                    response.Wait();
                    if (response.Result.StatusCode==System.Net.HttpStatusCode.OK)
                    {
                        this.DisplayToastAsync("Thao tác thành công", 1000);
                        this.ReFreshView.IsRefreshing = true;
                    }   
                    else
                    {
                        this.DisplayToastAsync("Thao tác thất bại", 1000);
                    }    
                }
                catch (Exception)
                {
                    this.DisplayToastAsync("Thao tác thất bại", 1000);
                }
            }    
        }

        private async void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Chọn hành động", "Cancel", null, "Chuyển video sang chế độ công khai");
            if (action == "Chuyển video sang chế độ công khai")
            {
                var video = (Image)sender;
                try
                {
                    HttpClient httpClient = new HttpClient();
                    //fetch infomation
                    var content = new Dictionary<string, string> { { "url", video.ClassId } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/PublicVideo", body);
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        this.DisplayToastAsync("Thao tác thành công", 1000);
                        this.ReFreshView.IsRefreshing = true;
                    }
                    else
                    {
                        this.DisplayToastAsync("Thao tác thất bại", 1000);
                    }
                }
                catch (Exception)
                {
                    this.DisplayToastAsync("Thao tác thất bại", 1000);
                }
            }
        }
    }
}