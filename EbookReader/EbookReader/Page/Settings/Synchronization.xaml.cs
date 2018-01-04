using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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

            IocManager.Container.Resolve<IMessageBus>().Subscribe<OpenDropboxLogin>(OpenDropboxLogin);
        }

        private async void OpenDropboxLogin(OpenDropboxLogin msg) {
            await Navigation.PushModalAsync(new SyncDropbox());
        }

    }
}