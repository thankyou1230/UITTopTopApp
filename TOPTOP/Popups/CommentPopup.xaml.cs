using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.MainTabs;
using TOPTOP.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public string videoUrl;
        public List<CommentDetails> videoComments;
        public User currentUser;
        public bool isCommenting = false;
        public string videoOwner;
        public CommentPopup()
        {
            InitializeComponent();
        }
        public CommentPopup(string videourl, string videoOwner, User currentUser)
        {
            InitializeComponent();
            this.videoUrl = videourl;
            this.currentUser = currentUser;
            this.videoOwner = videoOwner;
            FetchComments(videoUrl);
        }
        public bool FetchComments(string videoUrl)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "videourl", videoUrl } };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/GetVideoComments", body);
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                this.videoComments = JsonConvert.DeserializeObject<List<CommentDetails>>(responseContent.Result);
                commentsList.ItemsSource = this.videoComments;
                commentCountLabel.Text = this.videoComments.Count().ToString() + " bình luận";
                return true;
            }
            catch (Exception e)
            {
                this.DisplayToastAsync("Hiện tại không thể lấy bình luận", 2000);
                return false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                Label nickName = (Label)sender;
                var selectedComment = this.videoComments.Where(comment => comment.User.Email == nickName.ClassId).FirstOrDefault();
                if (this.currentUser.Email == selectedComment.User.Email)
                {
                    this.DisplayToastAsync("Người này chính là bạn đó", 1000);
                }
                else
                {
                    Navigation.PushAsync(new OtherProfilePage(this.currentUser.Email, selectedComment.User));
                }
            }
            catch (Exception)
            {
                Image nickName = (Image)sender;
                var selectedComment = this.videoComments.Where(comment => comment.User.Email == nickName.ClassId).FirstOrDefault();
                if (this.currentUser.Email == selectedComment.User.Email)
                {
                    this.DisplayToastAsync("Người này chính là bạn đó", 1000);
                }
                else
                {
                    Navigation.PushAsync(new OtherProfilePage(this.currentUser.Email, selectedComment.User));
                }
            }
        }

        private void SendCommentBtn_Clicked(object sender, EventArgs e)
        {
            string isOwner = (this.currentUser.Email == this.videoOwner) ? "True" : "False";
            //Save comment to server
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "videourl", videoUrl }, { "commenter", this.currentUser.Email}, { "content", CommentEntry.Text}, { "isowner", isOwner} };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/AddComment", body);
                response.Wait();
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var newComment = new Comment(this.currentUser.Email, this.videoUrl, CommentEntry.Text, DateTime.Now, isOwner);
                    this.videoComments.Insert(0, new CommentDetails(newComment, this.currentUser));
                    commentsList.ItemsSource = new List<CommentDetails>();
                    commentsList.ItemsSource = videoComments;
                }   
                else
                {
                    this.DisplayToastAsync("Đã có lỗi xảy ra", 1000);
                }    
            }
            catch (Exception)
            {
                this.DisplayToastAsync("Đã có lỗi xảy ra", 1000);
            }
            CommentEntry.Text = "";
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CommentEntry.Text!="")
            {
                SendCommentBtn.IsEnabled = true;
            }
            else
            {
                SendCommentBtn.IsEnabled = false;
            }
        }
    }
}