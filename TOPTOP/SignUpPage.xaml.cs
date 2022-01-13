using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        private bool isOnSignUp = false;
        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            emailStatus.TextColor = Color.Black;
            nicknameStatus.TextColor = Color.Black;
            passwordStatus.TextColor = Color.Black;
            confirmedPasswordStatus.TextColor = Color.Black;
            userNickname.Text = "";
            userEmail.Text = "";
            password.Text = "";
            confirmedPassword.Text = "";
        }
        public bool validateEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        private async void signUpBtn_Clicked(object sender, EventArgs e)
        {
            if(checkFormat())
            {
                //Wake up animation
                signUpBtn.IsEnabled = false;
                isOnSignUp = true;
                onSignUpAnimation();
                //Send verify request
                System.Net.HttpStatusCode signUpResult = await SignUpRequest(userEmail.Text, password.Text,userNickname.Text);
                isOnSignUp = false;
                signUpBtn.IsEnabled = true;
                if (signUpResult == System.Net.HttpStatusCode.OK)
                {
                    //announce and return to mainpage
                    await DisplayAlert("Thông báo", "Đăng ý thành công. Xin hãy xác nhận email để có thể sử dụng.", "Quay lại đăng nhập");
                    await Navigation.PopAsync();
                }
                else if (signUpResult == System.Net.HttpStatusCode.BadRequest)
                {
                    var _ = DisplayAlert("Thông báo", "Đã có lỗi xảy ra. Xin thử lại", "OK");
                }
                else if (signUpResult == System.Net.HttpStatusCode.Conflict)
                {
                    var _ = DisplayAlert("Thông báo", "Email đã được sử dụng", "OK");
                }
            }   
        }
        private bool checkFormat()
        {
            bool isRightFormat = true;
            if (userEmail.Text.Length > 0 && userNickname.Text.Length > 0)
            {
                //Check email format
                if (!validateEmail(userEmail.Text))
                {
                    emailStatus.Text = "Vui lòng nhập một email";
                    emailStatus.TextColor = Color.Red;
                    isRightFormat = false;
                }
                else
                {
                    emailStatus.TextColor = Color.Black;
                }
                //Check password lenght(6)
                if (password.Text.Length < 6)
                {
                    passwordStatus.Text = "Tối thiểu 6 ý tự";
                    passwordStatus.TextColor = Color.Red;
                    isRightFormat = false;
                }
                else
                {
                    passwordStatus.TextColor = Color.Black;
                }
                //Check confirm similarity
                if (password.Text == confirmedPassword.Text)
                {
                    confirmedPasswordStatus.TextColor = Color.Black;
                }
                else
                {
                    confirmedPasswordStatus.Text = "Mật khẩu xác nhận không trùng khớp";
                    confirmedPasswordStatus.TextColor = Color.Red;
                    isRightFormat = false;
                }
                return isRightFormat;
            }
            else
            {
                _ = DisplayAlert("Cảnh báo", "Xin vui lòng điền đầy đủ tất cả thông tin", "OK");
                return false;
            }
        }
        private async Task onSignUpAnimation()
        {
            while (isOnSignUp)
            {
                signUpBtn.Text = "Đang xử lí .";
                await Task.Delay(500);
                signUpBtn.Text = "Đang xử lí ..";
                await Task.Delay(500);
                signUpBtn.Text = "Đang xử lí ..";
                await Task.Delay(500);
            }
            signUpBtn.Text = "Đăng ký";
        }
        private async Task<System.Net.HttpStatusCode> SignUpRequest(string email, string password, string nickname)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new Dictionary<string, string> { { "email", email }, { "password", password },{ "nickname",nickname} };
                var body = new FormUrlEncodedContent(content);
                var response = await httpClient.PostAsync("https://toptop.azurewebsites.net/SignUp", body);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Policy());
        }

        private void userNickname_TextChanged(object sender, TextChangedEventArgs e)
        {
            userNickname.Text = userNickname.Text.Replace(" ", "");
        }
    }
}