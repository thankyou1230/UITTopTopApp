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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.Extensions;
using System.Diagnostics;
using Xamarin.CommunityToolkit.UI.Views;

namespace TOPTOP.MainTabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoTab : ContentPage
    {
        public bool isPopUpDisplaying = false;
        public List<VideoWithOwnerDetails> NewVideos;
        public string currentUser;
        public User currentUserProfile;
        public bool onlyFollowing;
        public VideoTab()
        {
            InitializeComponent();
        }
        public VideoTab(User currentUser, string videoUrl)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.currentUser = currentUser.Email;
            FetchSpecifedVideo(currentUser.Email, videoUrl);
            this.currentUserProfile = currentUser;
            RefreshingView.IsEnabled = false;
            switchView.IsVisible = false;
            onlyFollowing = false;
        }
        public VideoTab(User currentUser)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.currentUser = currentUser.Email;
            onlyFollowing = false;
            RefreshingView.IsRefreshing = true;
            this.currentUserProfile = currentUser;
            backBtn.IsVisible = false;
        }
        public bool FetchVideos(string email)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "email", email } };
                var body = new FormUrlEncodedContent(content);
                Task<HttpResponseMessage> response;
                if(this.onlyFollowing==false)
                {
                    response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetNewVideos", body);
                }
                else
                {
                    response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetNewFollowingVideos", body);
                }    
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                this.NewVideos = JsonConvert.DeserializeObject<List<VideoWithOwnerDetails>>(responseContent.Result);
                videosList.ItemsSource = this.NewVideos;
                return true;
            }
            catch (Exception e)
            {
                videosList.DisplayToastAsync("Không thể kết nối tới máy chủ", 1000);
                return false;
            }
        }
        public bool FetchSpecifedVideo(string email, string videoUrl)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "email", email }, { "videourl", videoUrl} };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetSpecifedVideo", body);
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                this.NewVideos = JsonConvert.DeserializeObject<List<VideoWithOwnerDetails>>(responseContent.Result);
                videosList.ItemsSource = this.NewVideos;
                return true;
            }
            catch (Exception e)
            {
                videosList.DisplayToastAsync("Không thể kết nối tới máy chủ", 1000);
                return false;
            }
        }

        private void videosList_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //Dừng video trước đó
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e) //like tap
        {
            try
            {
                var likeBtn = (Grid)sender;
                likeBtn.Children.RemoveAt(1);
                var thisVideo = NewVideos.Single(video => video.VideoDetails.Url == likeBtn.ClassId);
                if (thisVideo.VideoDetails.IsLiked == "True")
                {
                    //change icon
                    Image heart = new Image();
                    heart.Source = "notlike.png";
                    heart.WidthRequest = 30;
                    heart.HeightRequest = 35;
                    heart.Aspect = Aspect.AspectFit;
                    likeBtn.Children.Add(heart, 0, 0);
                    //change local data
                    thisVideo.VideoDetails.IsLiked = "False";
                    StackLayout parent = (StackLayout)likeBtn.Parent;
                    Label likeCount = (Label)parent.Children[2];
                    thisVideo.VideoDetails.LikeCount -= 1;
                    likeCount.Text = thisVideo.VideoDetails.LikeCount.ToString();
                    //change in server
                    HttpClient httpClient = new HttpClient();
                    var content = new Dictionary<string, string> { { "user", this.currentUser }, { "videourl", likeBtn.ClassId } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/UnLikeVideo", body);
                    response.Wait();
                }
                else
                {
                    //change icon
                    Image heart = new Image();
                    heart.Source = "liked.png";
                    heart.WidthRequest = 30;
                    heart.HeightRequest = 35;
                    heart.Aspect = Aspect.AspectFit;
                    likeBtn.Children.Add(heart, 0, 0);
                    //change local data
                    thisVideo.VideoDetails.IsLiked = "True";
                    thisVideo.VideoDetails.LikeCount += 1;
                    StackLayout parent = (StackLayout)likeBtn.Parent;
                    Label likeCount = (Label)parent.Children[2];
                    likeCount.Text = thisVideo.VideoDetails.LikeCount.ToString();
                    //change in server
                    HttpClient httpClient = new HttpClient();
                    var content = new Dictionary<string, string> { { "user", this.currentUser }, { "videourl", likeBtn.ClassId } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/LikeVideo", body);
                    response.Wait();
                }
            }
            catch(Exception)
            {
                videosList.DisplayToastAsync("Lỗi xử lí, xin hãy thao tác chậm lại", 1500);
            }

        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Image a = (Image)sender;
            var owner = this.NewVideos.Where(video => video.VideoDetails.Url == a.ClassId).FirstOrDefault().VideoDetails.Owner;
            if (!this.isPopUpDisplaying)
            {
                Navigation.PushPopupAsync(new CommentPopup(a.ClassId, owner, this.currentUserProfile));
            }
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Image a = (Image)sender;
            Navigation.PushPopupAsync(new SharePopup(a.ClassId));
        }

        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            //Move to profile page of owner
            Label nickName = (Label)sender;
            var selectedVideo = this.NewVideos.Where(video => video.VideoDetails.Url == nickName.ClassId).FirstOrDefault();
            if (this.currentUser==selectedVideo.OwnerDetails.Email)
            {
                this.DisplayToastAsync("Người này chính là bạn đó", 1000);
            }    
            else
            {
                Navigation.PushAsync(new OtherProfilePage(this.currentUser, selectedVideo.OwnerDetails));
            }    
        }

        private void RefreshingView_Refreshing(object sender, EventArgs e)
        {
            FetchVideos(currentUser);
            RefreshingView.IsRefreshing = false;
        }

        private void avatar_Clicked(object sender, EventArgs e)
        {
            ImageButton nickName = (ImageButton)sender;
            var selectedVideo = this.NewVideos.Where(video => video.VideoDetails.Url == nickName.ClassId).FirstOrDefault();
            if (this.currentUser == selectedVideo.OwnerDetails.Email)
            {
                this.DisplayToastAsync("Người này chính là bạn đó", 1000);
            }
            else
            {
                Navigation.PushAsync(new OtherProfilePage(this.currentUser, selectedVideo.OwnerDetails));
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e) //pause//play
        {
            Grid current = (Grid)sender;
            MediaElement child = (MediaElement)current.Children[0];
            if (current.ClassId=="True")
            {
                child.Pause();
                current.ClassId = "False";
            }
            else
            {
                child.Play();
                Grid a = new Grid();
                current.ClassId = "True";
            }

        }

        private void TapGestureRecognizer_Tapped_6(object sender, EventArgs e)
        {
            Grid current = (Grid)sender;
            var a = current.Children;
            FlexLayout child = (FlexLayout)current.Children[1];
            StackLayout childOfChild = (StackLayout)child.Children[0];
            Grid childOfChildOfChild = (Grid)childOfChild.Children[1];
            TapGestureRecognizer_Tapped(childOfChildOfChild, e);
        }

        private void TapGestureRecognizer_Tapped_7(object sender, EventArgs e)
        {
            OnlyFollowingOption.Opacity = 1;
            NewestOption.Opacity = 0.3;
            onlyFollowing = true;
            RefreshingView.IsRefreshing = true;
        }

        private void TapGestureRecognizer_Tapped_8(object sender, EventArgs e)
        {
            OnlyFollowingOption.Opacity = 0.3;
            NewestOption.Opacity = 1;
            onlyFollowing = false;
            RefreshingView.IsRefreshing = true;
        }
    }

    public struct Constant
    {
        public static double ScreenWidth = Application.Current.MainPage.Width;
        public static double ScreenHeight = Application.Current.MainPage.Height - 30;
    }

}