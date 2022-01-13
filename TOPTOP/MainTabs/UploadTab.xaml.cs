using Android.Graphics;
using Android.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.MainTabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadTab : ContentPage
    {
        public string currentUser;
        public UploadTab()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public UploadTab(string userEmail)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.currentUser = userEmail;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var videoPicker = await MediaPicker.CaptureVideoAsync(new MediaPickerOptions
                {
                    Title = "Quay video TopTop"
                });
                if (videoPicker!=null)
                {
                    var typingTitle = await DisplayPromptAsync(null, "Mô tả video của bạn:", "Tải lên", "Hủy bỏ","Hãy nói gì đó về video này",150);
                    if(typingTitle != "" && typingTitle != null)
                    {
                        this.DisplayToastAsync("Đang tải lên video", 1000);
                        var client = new HttpClient();
                        HttpResponseMessage response = new HttpResponseMessage();
                        HttpContent userEmail = new StringContent(this.currentUser);
                        HttpContent videoTitle = new StringContent(typingTitle);
                        var videoStream = await videoPicker.OpenReadAsync();
                        HttpContent fileStreamContent = new StreamContent(videoStream);
                        HttpContent thumbStreamContent = new StreamContent(GenerateThumbnail(videoPicker.FullPath));
                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(userEmail, "owner");
                            formData.Add(videoTitle, "title");
                            formData.Add(fileStreamContent, "video", videoPicker.FileName);
                            formData.Add(thumbStreamContent, "thumbnail", videoPicker.FileName.Replace(".mp4","") + "Thumbnail.jpeg");
                            response = await client.PostAsync("https://toptop.azurewebsites.net/UploadVideo", formData);
                            if(response.StatusCode==System.Net.HttpStatusCode.OK)
                            {
                                this.DisplayToastAsync("Tải lên thành công", 1000);
                            }    
                            else
                            {
                                this.DisplayToastAsync("Tải lên không thành công", 1000);
                            }   
                        }
                    }   
                    else
                    {
                        await this.DisplayToastAsync("Đã hủy bỏ tải lên video", 1000);
                    }    
                }   
                else
                {
                    await this.DisplayToastAsync("Không thể ghi nhận video", 1000);
                }    
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await this.DisplayToastAsync("Không thể quay video trên thiết bị này", 1000);
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
        public MemoryStream GenerateThumbnail(string path)
        {
            //When user cancel;
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(path);
            Bitmap bitmap = retriever.GetFrameAtTime(1);
            if (bitmap != null)
            {
                var stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 1, stream);
                byte[] bitmapData = stream.ToArray();
                return new MemoryStream(bitmapData);
            }
            return null;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            try
            {
                var videoPicker = await MediaPicker.PickVideoAsync(new MediaPickerOptions
                {
                    Title = "Chọn video muốn tải lên TopTop"
                });
                //Avatar.Source = ImageSource.FromStream(() => stream);
                await Navigation.PushAsync(new PreviewUpload(this.currentUser, videoPicker));
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await this.DisplayToastAsync("Không thể tải lên video trên thiết bị này", 1000);
            }
            catch (PermissionException pEx)
            {
                await this.DisplayToastAsync("Không có quyền truy cập bộ nhớ thiết bị", 1000);
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}