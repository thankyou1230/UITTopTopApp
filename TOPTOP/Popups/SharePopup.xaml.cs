using Android;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SharePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public string videoUrl;
        public SharePopup()
        {
            InitializeComponent();
        }
        public SharePopup(string videoUrl)
        {
            InitializeComponent();
            this.videoUrl = videoUrl;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DownloadVideo();
        }
        public async Task DownloadVideo()
        {
            try
            {
                await this.DisplayToastAsync("Đang tiến hành tải video", 500);
                string url = this.videoUrl;
                using (var client = new WebClient())
                {
                    var content = client.DownloadData(url);
                    var stream = new MemoryStream(content);

                    var context = Android.App.Application.Context;
                    var moviePath = context.GetExternalMediaDirs()[0];
                    var storagePath = Path.Combine("/storage/emulated/0/DCIM/Camera", "TOPTOP" + DateTime.Now.ToString().Replace(" ", "").Replace(":", "").Replace("/", "") + ".mp4");
                    using (var outputStream = new FileStream(storagePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await stream.CopyToAsync(outputStream);
                        this.DisplayToastAsync("Đã tải xong", 500);
                    }
                }
            }
            catch (Exception ex)
            {
                this.DisplayToastAsync("Không lưu được video, xin thử lại", 1500);
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e) //get lin
        {
            var _ = Clipboard.SetTextAsync(videoUrl);
            _.Wait();
            this.DisplayToastAsync("Đã copy link vào bộ nhớ tạm", 1000);

        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            //report tap
        }
    }
}