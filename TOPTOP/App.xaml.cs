using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TOPTOP.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LocalDatabase db = new LocalDatabase();
            bool _ = db.CreateLocalDatabase();
        }

        protected override void OnStart()
        {
            LocalDatabase db = new LocalDatabase();
            CurrentUser _ = db.GetCurrentUser();
            if (_!=null)
            {
                var currentUserProfile = FetchProfile(_.email);
                if (currentUserProfile != null)
                {
                    MainPage = new NavigationPage(new TabbedMainPage(currentUserProfile.MainProfile));
                }
                else
                {
                    //thông áo lỗi
                }    
                //MainPage = new NavigationPage(new MainPage(_.email));
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
        public Profile FetchProfile(string email)
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
                var profile = JsonConvert.DeserializeObject<Profile>(responseContent.Result);
                return profile;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
