using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EbookReader.Model.View;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage {
        public AboutPage() {
            InitializeComponent();

            BindingContext = new AboutVM();

            var source = new HtmlWebViewSource {
                Html = this.GetFlaticonHTML()
            };

            Flaticon.Source = source;
            Flaticon.Navigating += Flaticon_Navigating;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Flaticon_Navigating(object sender, WebNavigatingEventArgs e) {
            e.Cancel = true;
            try {
                Device.OpenUri(new Uri(e.Url));
            } catch (Exception) { }
        }

        private string GetFlaticonHTML() {
            return @"
<div style='text-align: center'>
    <div>Icons made by <a href='https://www.flaticon.com/authors/gregor-cresnar' title='Gregor Cresnar'>Gregor Cresnar</a> from <a href='https://www.flaticon.com/' title='Flaticon'>www.flaticon.com</a> is licensed by <a href='http://creativecommons.org/licenses/by/3.0/' title='Creative Commons BY 3.0'>CC 3.0 BY</a></div>

    <div>Icons made by <a href='https://www.flaticon.com/authors/chris-veigt' title='Chris Veigt'>Chris Veigt</a> from <a href='https://www.flaticon.com/' title='Flaticon'>www.flaticon.com</a> is licensed by <a href='http://creativecommons.org/licenses/by/3.0/' title='Creative Commons BY 3.0'>CC 3.0 BY</a></div>

    <div>Icons made by <a href='https://www.flaticon.com/authors/good-ware' title='Good Ware'>Good Ware</a> from <a href='https://www.flaticon.com/' title='Flaticon'>www.flaticon.com</a> is licensed by <a href='http://creativecommons.org/licenses/by/3.0/' title='Creative Commons BY 3.0' target='_blank'>CC 3.0 BY</a></div>

    <div>Icons made by <a href='http://www.freepik.com' title='Freepik'>Freepik</a> from <a href='https://www.flaticon.com/' title='Flaticon'>www.flaticon.com</a> is licensed by <a href='http://creativecommons.org/licenses/by/3.0/' title='Creative Commons BY 3.0' target='_blank'>CC 3.0 BY</a></div>
</div>
            ";
        } 
    }
}