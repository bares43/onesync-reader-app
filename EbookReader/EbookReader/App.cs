using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using EbookReader.Page;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        static HomePage homePage;
        static SettingsPage settingsPage;

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

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
