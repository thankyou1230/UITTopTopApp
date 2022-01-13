using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using Xamarin.CommunityToolkit.Extensions;
//using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IGoogleManager _googleManager;
        GoogleUser GoogleUser = new GoogleUser();
        private bool isOnLogin = false; 
        public LoginPage()
        {
            _googleManager = DependencyService.Get<IGoogleManager>();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            userID.Text = "";
            password.Text = "";

        }
        private void CheckUserLoggedIn()
        {
            //_googleManager.Login(OnLoginComplete);
        }
        private void OnLoginComplete(GoogleUser googleUser, string message)
        {
            if (googleUser != null)
            {
                Indicator.IsVisible = true;
                GoogleUser = googleUser;
                //Store signed in user to local db
                CurrentUser temp = new CurrentUser() { email = GoogleUser.Email };
                LocalDatabase db = new LocalDatabase();
                db.AddCurrentUser(temp);
                //Navigate to home page
                var previousPage = Navigation.NavigationStack.LastOrDefault();
                var profile = FetchGoogleProfile(GoogleUser);
                Indicator.IsVisible = false;
                if (profile!=null)
                {
                    Navigation.PushAsync(new TabbedMainPage(profile.MainProfile));
                    Navigation.RemovePage(previousPage);
                }
                else
                {
                    this.DisplayToastAsync("Không thể kết nối tới máy chủ", 2000);
                }    
                //Maybe auto signup for this email
                //*******
                //******
            }
            else
            {
                this.DisplayToastAsync(message, 1000);
            }
        }
        public Profile FetchGoogleProfile(GoogleUser user)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "email", user.Email },{ "avatar",user.Picture.ToString()},{ "nickname", user.Name }};
                var body = new FormUrlEncodedContent(content);
                var response = httpClient.PostAsync("https://toptop.azurewebsites.net/FetchGoogleProfile", body);
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
        public Profile FetchProfile(string user)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //fetch infomation
                var content = new Dictionary<string, string> { { "email", user }};
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignUpPage());
        }

        private async void SignInBtn_Clicked(object sender, EventArgs e)
        {
            if (userID.Text!="" && password.Text!="")
            {
                //Wake up animation
                SignInBtn.IsEnabled = false;
                isOnLogin = true;
                onLoginAnimation();
                //Send verify request
                System.Net.HttpStatusCode verifyResult = await VerifyAccount(userID.Text, password.Text);
                isOnLogin = false;
                SignInBtn.IsEnabled = true;
                //Handle the reply from server
                if (verifyResult == System.Net.HttpStatusCode.OK)
                {
                    //update current user
                    LocalDatabase db = new LocalDatabase();
                    db.AddCurrentUser(new CurrentUser() { email=userID.Text});
                    //navigate to mainpage
                    var previousPage = Navigation.NavigationStack.LastOrDefault();
                    var profile = FetchProfile(userID.Text);
                    if (profile != null)
                    {
                        await Navigation.PushAsync(new TabbedMainPage(profile.MainProfile));
                        Navigation.RemovePage(previousPage);
                    }
                    else
                    {
                        await this.DisplayToastAsync("Không thể kết nối tới máy chủ", 2000);
                    }
                }
                else if (verifyResult == System.Net.HttpStatusCode.BadRequest)
                {
                    LoginStatus.Text = "*Sai thông tin đăng nhập";
                    LoginStatus.TextColor = Color.Red;
                }
                else if (verifyResult == System.Net.HttpStatusCode.Unauthorized)
                {
                    LoginStatus.Text = "*Email chưa được xác thực";
                    LoginStatus.TextColor = Color.Red;
                }    
                else
                {
                    var _ = DisplayAlert("Cảnh báo", "Đã có lỗi hệ thống xảy ra", "OK");
                }    
            }
            else
            {
                //var _ = DisplayAlert("Cảnh báo", "Xin nhập đầy đủ thông tin đăng nhập", "OK");
            }
        }
        private async Task onLoginAnimation()
        {
            while(isOnLogin)
            {
                SignInBtn.Text = "Đang đăng nhập .";
                await Task.Delay(500);
                SignInBtn.Text = "Đang đăng nhập ..";
                await Task.Delay(500);
                SignInBtn.Text = "Đang đăng nhập ..";
                await Task.Delay(500);
            }
            SignInBtn.Text = "Đăng nhập";
        }

        private async Task<System.Net.HttpStatusCode> VerifyAccount(string email, string password)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new Dictionary<string, string> { { "email", email }, { "password", password } };
                var body = new FormUrlEncodedContent(content);
                var response = await httpClient.PostAsync("https://toptop.azurewebsites.net/Login", body);
                return response.StatusCode;
            }
            catch(Exception ex)
            {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ResetPasswordPage());
        }

        private void GoogleSignInBtn_Clicked(object sender, EventArgs e)
        {
            _googleManager.Logout();
            _googleManager.Login(OnLoginComplete);
        }
    }
}