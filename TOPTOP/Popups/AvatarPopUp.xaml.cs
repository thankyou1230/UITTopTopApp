using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TOPTOP.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AvatarPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public AvatarPopUp()
        {
            InitializeComponent();
        }
        public AvatarPopUp(string avatar)
        {
            InitializeComponent();
            this.avatar.Source = avatar;
        }
    }
}