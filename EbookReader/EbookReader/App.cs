using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        public App() {
       
            var webView = new FormsWebView() {
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.Internet,
                Source = "https://www.google.cz/",
                WidthRequest = 500,
                HeightRequest = 500
            };


            var content = new ContentPage {
                Title = "EbookReader",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        webView
                    }
                }
            };

            MainPage = new NavigationPage(content);
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
