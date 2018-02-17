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
using EbookReader.Model.Messages;

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

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e) {
            if (UserSettings.Control.VolumeButtons && keyCode == Keycode.VolumeDown || keyCode == Keycode.VolumeUp) {
                var messageBus = IocManager.Container.Resolve<IMessageBus>();
                messageBus.Send(new GoToPage { Next = keyCode == Keycode.VolumeDown, Previous = keyCode == Keycode.VolumeUp });

                return true;
            }

            return base.OnKeyDown(keyCode, e);
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
            messageBus.Subscribe<Model.Messages.FullscreenRequest>(ToggleFullscreen);
        }

        private void ChangeBrightness(Model.Messages.ChangesBrightness msg) {
            RunOnUiThread(() => {
                var brightness = Math.Min(msg.Brightness, 1);
                brightness = Math.Max(brightness, 0);

                var attributesWindow = new WindowManagerLayoutParams();
                attributesWindow.CopyFrom(Window.Attributes);
                attributesWindow.ScreenBrightness = brightness;
                Window.Attributes = attributesWindow;
            });
        }

        private void ToggleFullscreen(Model.Messages.FullscreenRequest msg) {
            if (msg.Fullscreen) {
                RunOnUiThread(() => {
                    Window.AddFlags(WindowManagerFlags.Fullscreen);
                });
            } else {
                RunOnUiThread(() => {
                    Window.ClearFlags(WindowManagerFlags.Fullscreen);
                });
            }
        }


    }
}

