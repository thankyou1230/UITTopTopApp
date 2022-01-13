using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPTOP.MainTabs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Platform.Android;
using TOPTOP.Models;

namespace TOPTOP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedMainPage : Xamarin.Forms.TabbedPage
    {
        public User currentUser;
        public TabbedMainPage()
        {
            InitializeComponent();
        }
        public TabbedMainPage(User currentUserProfile)
        {
            this.currentUser = currentUserProfile;
            //init video page
            var videoPage = new NavigationPage(new VideoTab(currentUserProfile));
            videoPage.IconImageSource = "homeicon.png";
            videoPage.Title = "Trang chủ";
            Children.Add(videoPage);
            //init explore page
            var explorePage = new NavigationPage(new ExploreTab(currentUserProfile));
            explorePage.IconImageSource = "exploreicon.png";
            explorePage.Title = "Khám phá";
            Children.Add(explorePage);
            //init add button
            var addVideoPage = new NavigationPage(new UploadTab(currentUserProfile.Email));
            addVideoPage.IconImageSource = "addition.png";
            Children.Add(addVideoPage);
            //init noti page
            var notiPage = new NavigationPage(new NotiTab(currentUserProfile.Email));
            notiPage.IconImageSource = "notifyicon.png";
            notiPage.Title = "Thông báo";
            Children.Add(notiPage);
            //init profile page
            var profilePage = new NavigationPage(new ProfileTab(currentUserProfile.Email));
            profilePage.IconImageSource = "profileicon.png";
            profilePage.Title = "Hồ sơ";
            Children.Add(profilePage);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

    }
}