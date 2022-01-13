using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.MainTabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploreTab : ContentPage
    {
        public User currentUser;
        public SearchResult searchResult;
        public ExploreTab()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
		}
        public ExploreTab(User currentUserProfile)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.currentUser = currentUserProfile;
        }
        public bool FetchSearchResult(string searchText)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "searchtext", searchText }, { "user", this.currentUser.Email} };
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/Search", body);
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                this.searchResult = JsonConvert.DeserializeObject<SearchResult>(responseContent.Result);
                resultUserView.ItemsSource = this.searchResult.UserResult;
                resultVideoView.ItemsSource = this.searchResult.VideoResult;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            this.Indicator.IsVisible = true;
            FetchSearchResult(SearchInput.Text);
            this.Indicator.IsVisible = false;
            resultVideoView.IsVisible = true;
            resultUserView.IsVisible = true;

        }

        private void resultUserView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultUserView.SelectedItem != null)
            {
                var selectedUser = (MatchedUserResult)resultUserView.SelectedItem;
                Navigation.PushAsync(new OtherProfilePage(this.currentUser.Email, selectedUser.Profile));
                resultUserView.SelectedItem = null;
            }
        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchInput.Text!=null)
            {
                SearchBtn.IsEnabled = true;
            }    
            else
            {
                SearchBtn.IsEnabled = false;
            }    
        }

        private void resultVideoView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultVideoView.SelectedItem != null)
            {
                var selectedVideo = (MatchedVideoResult)resultVideoView.SelectedItem;
                Navigation.PushAsync(new VideoTab(currentUser, selectedVideo.Video.Url));
                resultVideoView.SelectedItem = null;
            }
        }
    }
}