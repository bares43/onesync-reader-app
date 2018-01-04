using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dropbox.Api;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Settings {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SyncDropbox : ContentPage {

        string dropboxClientID = "wk719mekght88r6";

        public SyncDropbox() {
            InitializeComponent();

            BindingContext = new {
                Url = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, dropboxClientID, "https://janbares.cz/").AbsoluteUri
            };
        }

        private async void AuthWebView_OnNavigationStarted(object sender, Xam.Plugin.WebView.Abstractions.Delegates.DecisionHandlerDelegate e) {
            if (!e.Uri.StartsWith("https://www.dropbox") && e.Uri.Contains("janbares")) {
                var uri = new Uri(e.Uri);
                var accessToken = DropboxOAuth2Helper.ParseTokenFragment(uri).AccessToken;
                IocManager.Container.Resolve<IMessageBus>().Send(new DropboxAccessTokenMessage { AccessToken = accessToken });
                await Navigation.PopModalAsync();
            }
        }
    }
}