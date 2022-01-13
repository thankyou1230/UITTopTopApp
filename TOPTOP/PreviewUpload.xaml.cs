using Android.Graphics;
using Android.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviewUpload : ContentPage
    {
        public string currentUser;
        public FileResult video;
        public PreviewUpload()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public PreviewUpload(string currentUser, FileResult video)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.currentUser = currentUser;
            this.video = video;
            LoadVideoToPreview(video);
        }
        public void LoadVideoToPreview(FileResult video)
        {
            if(video!=null)
            {
                this.VideoPreview.Source = MediaSource.FromFile(video.FullPath);
            }   
            else
            {
                this.DisplayToastAsync("Đã có lỗi xảy ra", 1000);
            }    
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (this.video != null)
                {
                    var typingTitle = await DisplayPromptAsync(null, "Mô tả video của bạn:", "Tải lên", "Hủy bỏ", "Hãy nói gì đó về video này", 150);
                    if (typingTitle != "" && typingTitle != null)
                    {
                        this.DisplayToastAsync("Đang tải lên video", 1000);
                        var client = new HttpClient();
                        HttpResponseMessage response = new HttpResponseMessage();
                        HttpContent userEmail = new StringContent(this.currentUser);
                        HttpContent videoTitle = new StringContent(typingTitle);
                        var videoStream = await this.video.OpenReadAsync();
                        HttpContent fileStreamContent = new StreamContent(videoStream);
                        HttpContent thumbStreamContent = new StreamContent(GenerateThumbnail(this.video.FullPath));
                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(userEmail, "owner");
                            formData.Add(videoTitle, "title");
                            formData.Add(fileStreamContent, "video", this.video.FileName);
                            formData.Add(thumbStreamContent, "thumbnail", this.video.FileName.Replace(".mp4", "") + "Thumbnail.jpeg");
                            response = await client.PostAsync("https://toptop.azurewebsites.net/UploadVideo", formData);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                this.DisplayToastAsync("Tải lên thành công", 1000);
                                Navigation.PopAsync();
                            }
                            else
                            {
                                this.DisplayToastAsync("Tải lên KHÔNG thành công", 1000);
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
            catch(Exception)
            {

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

    }
}