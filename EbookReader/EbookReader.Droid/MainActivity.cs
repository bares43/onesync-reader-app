using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using EbookReader.DependencyService;
using EbookReader.Droid.DependencyService;
using Autofac;
using Xam.Plugin.WebView.Droid;
using EbookReader.Service;

namespace EbookReader.Droid {
    [Activity(Label = "EbookReader", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
        protected override void OnCreate(Bundle bundle) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            this.SetUpIoc();
            this.SetUpSubscribers();

            FormsWebViewRenderer.Initialize();

            FormsWebViewRenderer.OnControlChanged += (sender, webView) => {
                webView.SetLayerType(LayerType.Software, null);
                webView.Settings.LoadWithOverviewMode = true;
                webView.Settings.UseWideViewPort = true;
            };

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
        
        private void SetUpIoc() {
            IocManager.ContainerBuilder.RegisterType<AndroidAssetsManager>().As<IAssetsManager>();
            IocManager.ContainerBuilder.RegisterType<BrightnessProvider>().As<IBrightnessProvider>();
            IocManager.ContainerBuilder.RegisterInstance(new BrightnessProvider { Brightness = Android.Provider.Settings.System.GetFloat(ContentResolver, Android.Provider.Settings.System.ScreenBrightness) / 255 }).As<IBrightnessProvider>();
            IocManager.ContainerBuilder.RegisterType<CryptoService>().As<ICryptoService>();
            IocManager.Build();
        }

        private void SetUpSubscribers() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Subscribe<Model.Messages.ChangesBrightness>(ChangeBrightness);
        }

        private void ChangeBrightness(Model.Messages.ChangesBrightness msg) {
            var attributesWindow = new WindowManagerLayoutParams();
            attributesWindow.CopyFrom(Window.Attributes);
            attributesWindow.ScreenBrightness = msg.Brightness;
            Window.Attributes = attributesWindow;
        }
    }
}

