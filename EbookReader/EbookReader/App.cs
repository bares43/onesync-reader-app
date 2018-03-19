using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Page;
using EbookReader.Service;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using EbookReader.DependencyService;

namespace EbookReader {
    public class App : Application {

        private IMessageBus _messageBus;

        private bool doubleBackToExitPressedOnce = false;

        public static bool HasMasterDetailPage {
            get {
                return Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android;
            }
        }

        public App() {
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            if (App.HasMasterDetailPage) {
                MainPage = new MasterDetailPage1();

            } else {
                MainPage = new NavigationPage(new HomePage());
            }

        }

        protected override void OnStart() {
            // Handle when your app starts
            AppCenter.Start($"android={AppSettings.AppCenter.Android};uwp={AppSettings.AppCenter.UWP};", typeof(Analytics), typeof(Crashes));
            Analytics.SetEnabledAsync(UserSettings.AnalyticsAgreement);

            _messageBus.UnSubscribe("App");
            _messageBus.Subscribe<BackPressedMessage>(BackPressedMessageSubscriber, new string[] { "App" });
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
            _messageBus.Send(new AppSleepMessage());
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }

        private async void BackPressedMessageSubscriber(BackPressedMessage msg) {
            var master = MainPage as MasterDetailPage1;

            if (master != null) {
                var detailPage = master.Detail.Navigation.NavigationStack.LastOrDefault();

                if (detailPage is ReaderPage readerPage && readerPage.IsQuickPanelVisible()) {
                    _messageBus.Send(new CloseQuickPanelMessage());
                } else if (detailPage is HomePage) {
                    if (doubleBackToExitPressedOnce) {
                        _messageBus.Send(new CloseAppMessage());
                    } else {
                        IocManager.Container.Resolve<IToastService>().Show("Press once again to exit!");
                        doubleBackToExitPressedOnce = true;
                        Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 2), () => {
                            doubleBackToExitPressedOnce = false;
                            return false;
                        });
                    }
                } else {
                    await master.Detail.Navigation.PopAsync();
                }
            }
        }
    }
}
