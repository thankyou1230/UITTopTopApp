using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailFollowPage : ContentPage
    {
        public string currentUser;
        public string listOwner;
        public DetailFollowPage()
        {
            InitializeComponent();
        }
        public DetailFollowPage(List<User> Followings, List<User> Followers, string currentUser)
        {
            InitializeComponent();
            FollowingView.ItemsSource = Followings;
            FollowerView.ItemsSource = Followers;
            this.currentUser = currentUser;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            HttpClient httpClient = new HttpClient();
            if (btn.Text=="Theo dõi")
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
                    if (response.Result.StatusCode==System.Net.HttpStatusCode.OK)
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

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Text = "Đang xử lí";
            btn.IsEnabled = false;
            HttpClient httpClient = new HttpClient();
            //Logic hủy theo dõi của người ta với mình
            try
            {
                string unfollowFrom = btn.ClassId;
                var content = new Dictionary<string, string> { { "UnfollowFrom", unfollowFrom }, { "UnfollowTo", currentUser } };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/UnfollowRequest", body);
                response.Wait();
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    btn.Text = "Đã xóa";
                    btn.BackgroundColor = Color.Black;
                    btn.FontAttributes = FontAttributes.Italic;
                }
                else
                {
                    btn.Text = "Xóa theo dõi";
                    btn.BackgroundColor = Color.FromHex("#FF66C4");
                    btn.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                btn.Text = "Xóa theo dõi";
                btn.BackgroundColor = Color.FromHex("#FF66C4");
                btn.IsEnabled = true;
            }
        }

        private void FollowingView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FollowingView.SelectedItem != null)
            {
                Navigation.PushAsync(new OtherProfilePage(this.currentUser, (User)FollowingView.SelectedItem));
                FollowingView.SelectedItem = null;
            }    
        }

        private void FollowerView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FollowerView.SelectedItem != null)
            {
                Navigation.PushAsync(new OtherProfilePage(this.currentUser, (User)FollowerView.SelectedItem));
                FollowerView.SelectedItem = null;
            }
        }
    }
}