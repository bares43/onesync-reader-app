using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model;
using EbookReader.Model.Messages;
using EbookReader.Model.View;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Settings {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Synchronization : ContentPage {

        SettingsSynchronizationVM vm;

        public Synchronization() {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.UWP) {
                Content.HorizontalOptions = LayoutOptions.Start;
                Content.WidthRequest = 500;
            }

            vm = new SettingsSynchronizationVM();

            BindingContext = vm;

            IocManager.Container.Resolve<IMessageBus>().Subscribe<OpenDropboxLoginMessage>(OpenDropboxLogin);

            IocManager.Container.Resolve<IMessageBus>().Subscribe(async (OAuth2AccessTokenObtainedMessage msg) => {
                if (msg.Provider == "Dropbox") {
                    await Navigation.PopModalAsync();
                }
            });
        }

        private async void OpenDropboxLogin(OpenDropboxLoginMessage msg) {

            var OAuth2Data = new OAuth2RequestData {
                Provider = "Dropbox",
                ClientID = AppSettings.Synchronization.Dropbox.ClientID,
                AuthorizeUrl = "https://www.dropbox.com/oauth2/authorize",
                RedirectUrl = AppSettings.Synchronization.Dropbox.RedirectUrl,
            };

            Xamarin.Forms.Application.Current.Properties["OAuth2Data"] = OAuth2Data;

            await Navigation.PushModalAsync(new OAuth2LoginPage());
        }

        private void Email_Completed(object sender, EventArgs e) {
            FirebasePassword.Focus();
        }

        private void Password_Completed(object sender, EventArgs e) {
            if (vm != null && vm.Firebase != null && vm.Firebase.ConnectCommand != null && !string.IsNullOrEmpty(vm.Firebase.Email) && !string.IsNullOrEmpty(vm.Firebase.Password)) {
                vm.Firebase.ConnectCommand.Execute(null);
            }
        }
    }
}