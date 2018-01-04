using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Page;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        static HomePage homePage;
        static SettingsPage settingsPage;
        static AboutPage aboutPage;

        public App() {
            MainPage = new NavigationPage(new HomePage());
        }

        public static HomePage HomePage() {
            if(homePage == null) {
                homePage = new HomePage();
            }

            return homePage;
        }
        
        public static SettingsPage SettingsPage() {
            if(settingsPage == null) {
                settingsPage = new SettingsPage();
            }

            return settingsPage;
        }

        public static AboutPage AboutPage() {
            if(aboutPage == null) {
                aboutPage = new AboutPage();
            }

            return aboutPage;
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
            IocManager.Container.Resolve<IMessageBus>().Send(new AppSleep());
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
