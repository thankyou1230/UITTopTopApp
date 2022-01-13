using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        public bool avatarFromLink;
        public bool avatarChanged;
        public bool nickNameChanged;
        public bool bioChanged;
        public User currentUser;
        public FileResult avatarPicker;
        public string avatarName;
        public string avatarLink;
        public EditProfilePage()
        {
            InitializeComponent();
        }
        public EditProfilePage(User currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            NickNameInput.Text = currentUser.NickName;
            BioInput.Text = currentUser.Bio;
            Avatar.Source = currentUser.Avatar;
            avatarFromLink = false;
            avatarChanged = false;
            nickNameChanged = false;
            bioChanged = false;
            avatarLink = "";
        }

        public void CheckChanged()
        {
            if(avatarChanged||nickNameChanged||bioChanged)
            {
                SaveChangeBtn.Text = "Lưu thay đổi";
            }
            else
            {
                SaveChangeBtn.Text = "Quay lại";
            }    
        }
        private async void ChangeAvatarBtn_Clicked(object sender, EventArgs e)
        {
            string[] options = new string[] { "Từ thiết bị", "Từ link" };
            if (avatarChanged==true)
            {
                options = new string[] { "Từ thiết bị", "Từ link", "Hủy thay đổi" };
            }    
            string action = await DisplayActionSheet("Chọn nguồn ảnh", "Quay lại", null, options);
            if (action=="Từ thiết bị")
            {
                try
                {
                    this.avatarPicker = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Chọn ảnh đại diện TopTop của bạn"
                    });
                    var stream = await this.avatarPicker.OpenReadAsync();
                    this.avatarName = this.avatarPicker.FileName;
                    Debug.WriteLine("Avatar Path: " + avatarPicker.FullPath);
                    Avatar.Source = ImageSource.FromStream(() => stream);
                    this.avatarChanged = true;
                    this.avatarFromLink = false;
                    CheckChanged();
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    await this.DisplayToastAsync("Không thể mở thư viện", 1000);
                }
                catch (PermissionException pEx)
                {
                   await this.DisplayToastAsync("Không có quyền truy cập bộ nhớ thiết bị", 1000);
                }
                catch (Exception ex)
                {
                    //When user cancel;
                }
            }
            else if(action=="Từ link")
            {
                string result = await DisplayPromptAsync("Từ link", "Nhập link ảnh của bạn");
                if (result != "")
                {
                    Avatar.Source = result;
                    this.avatarChanged = true;
                    this.avatarFromLink = true;
                    this.avatarLink = result;
                    CheckChanged();
                }    
            }
            else if(action=="Hủy thay đổi")
            {
                Avatar.Source = this.currentUser.Avatar;
                this.avatarChanged = false;
                this.avatarFromLink = false;
                CheckChanged();
            }    
        }

        private void NickNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(NickNameInput.Text == this.currentUser.NickName)
            {
                this.nickNameChanged = false;
            }  
            else
            {
                this.nickNameChanged = true;
            }
            CheckChanged();
        }

        private void BioInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BioInput.Text == this.currentUser.Bio)
            {
                this.bioChanged = false;
            }
            else
            {
                this.bioChanged = true;
            }
            CheckChanged();
        }
        private async void SaveChangetn_Clicked(object sender, EventArgs e)
        {
            SaveChangeBtn.IsEnabled = false;
            this.DisplayToastAsync("Đang cập nhật chỉnh sửa của bạn ...", 2000);
            try
            {
                if (SaveChangeBtn.Text == "Lưu thay đổi")
                {
                    if (NickNameInput.Text != "")
                    {
                        var client = new HttpClient();
                        HttpResponseMessage response = new HttpResponseMessage();
                        HttpContent emailContent = new StringContent(this.currentUser.Email);
                        HttpContent nickNameContent = new StringContent(NickNameInput.Text);
                        HttpContent bioContent = new StringContent(BioInput.Text);
                        if (!this.avatarChanged)
                        {
                            using (var formData = new MultipartFormDataContent())
                            {
                                HttpContent avatarContent = new StringContent(this.currentUser.Avatar);
                                formData.Add(emailContent, "email");
                                formData.Add(nickNameContent, "nickname");
                                formData.Add(bioContent, "bio");
                                formData.Add(avatarContent, "avatar");
                                response = await client.PostAsync("https://toptop.azurewebsites.net/UpdateProfileWithoutUploadAvatar", formData);
                            }
                        }
                        else if (avatarFromLink)
                        {
                            using (var formData = new MultipartFormDataContent())
                            {
                                HttpContent avatarContent = new StringContent(this.avatarLink);
                                formData.Add(emailContent, "email");
                                formData.Add(nickNameContent, "nickname");
                                formData.Add(bioContent, "bio");
                                formData.Add(avatarContent, "avatar");
                                response = await client.PostAsync("https://toptop.azurewebsites.net/UpdateProfileWithoutUploadAvatar", formData);
                            }
                        }
                        else
                        {
                            //request update
                            var avatarStream = await this.avatarPicker.OpenReadAsync();
                            HttpContent fileStreamContent = new StreamContent(avatarStream);
                            var formData = new MultipartFormDataContent();
                            formData.Add(emailContent, "email");
                            formData.Add(nickNameContent, "nickname");
                            formData.Add(bioContent, "bio");
                            formData.Add(fileStreamContent, "avatar", this.currentUser.Email + this.avatarName);
                            response = await client.PostAsync("https://toptop.azurewebsites.net/UpdateProfileWithUploadAvatar", formData);
                        }
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            await this.DisplayToastAsync("Cập nhật hồ sơ thành công", 1000);
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await this .DisplayToastAsync("Lỗi kết nối, vui lòng thử lại", 2000);
                        }
                    }
                    else
                    {
                        await this.DisplayToastAsync("Không được để trống biệt danh của bạn", 2000);
                        Error.IsVisible = true;
                        SaveChangeBtn.IsEnabled = true;
                    }
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
            catch(Exception)
            {
                await this.DisplayToastAsync("Lỗi kết nối tới server", 1000);
                SaveChangeBtn.IsEnabled = true;
            }
        }
    }
}