using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.MainTabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotiTab : ContentPage
    {
        public string currentUser;
        public List<Notification> currentNotis = new List<Notification>();
        public NotiTab()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public NotiTab(string currentUser)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            FetchNoti(currentUser).ConfigureAwait(false);
            this.currentUser = currentUser;
        }
        public async Task FetchNoti(string email)
        {

            while(true)
            {
                List<Notification> incomingNotis = new List<Notification>();
                try
                {
                    HttpClient httpClient = new HttpClient();
                    //fetch infomation
                    var content = new Dictionary<string, string> { { "email", email } };
                    var body = new FormUrlEncodedContent(content);
                    var response = httpClient.PostAsync("https://toptop.azurewebsites.net/FetchNoti", body);
                    response.Wait();
                    var responseContent = response.Result.Content.ReadAsStringAsync();
                    responseContent.Wait();
                    this.currentNotis = JsonConvert.DeserializeObject<List<Notification>>(responseContent.Result);
                    this.currentNotis.OrderBy(noti => noti.NotiTime);
                    notiList.ItemsSource = this.currentNotis;
                }
                catch (Exception)
                {
                    //
                }
                await Task.Delay(10000);
            }    
        }

        private void notiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notiList.SelectedItem != null)
            {
                Notification selected = (Notification)notiList.SelectedItem;
                Navigation.PushAsync(new OtherProfilePage(this.currentUser, selected.NotiFrom));
                notiList.SelectedItem = null;
            }
        }
    }
}