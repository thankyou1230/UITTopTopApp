using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResetPasswordPage : ContentPage
    {
        private bool isOnProcess = false;
        public ResetPasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            email.Text = "";
            status.IsVisible = false;
        }
        public bool validateEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(email.Text!="")
            {
                if (validateEmail(email.Text))
                {
                    ResetBtn.IsEnabled = false;
                    isOnProcess = true;
                    onProcessAnimation();
                    //Send reset request
                    System.Net.HttpStatusCode requestResult = await ResetRequest(email.Text);
                    isOnProcess = false;
                    ResetBtn.IsEnabled = true;
                    if(requestResult==System.Net.HttpStatusCode.OK)
                    {
                        await DisplayAlert("Thông báo", "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var _ = DisplayAlert("Thông báo", "Email không tồn tại. Xin thử lại", "OK");
                    }

                }
                else
                {
                    status.IsVisible = true;
                }    
            }   
            else
            {
                status.IsVisible = true;
            }    
        }
        private async Task<System.Net.HttpStatusCode> ResetRequest(string email)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new Dictionary<string, string> { { "email", email } };
                var body = new FormUrlEncodedContent(content);
                var response = await httpClient.PostAsync("https://toptop.azurewebsites.net/ResetPassword", body);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }
        private async Task onProcessAnimation()
        {
            while (isOnProcess)
            {
                ResetBtn.Text = "Đang xử lí .";
                await Task.Delay(500);
                ResetBtn.Text = "Đang xử lí ..";
                await Task.Delay(500);
                ResetBtn.Text = "Đang xử lí ..";
                await Task.Delay(500);
            }
            ResetBtn.Text = "Đặt lại mật khẩu";
        }
    }
}