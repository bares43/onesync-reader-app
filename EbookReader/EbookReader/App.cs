using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookReader.DependencyService;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        FormsWebView webView;

        public App() {

            webView = new FormsWebView() {
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData,
                WidthRequest = 500,
                HeightRequest = 500
            };

            this.LoadWebViewLayout();

            webView.OnContentLoaded += WebView_OnContentLoaded;

            webView.RegisterGlobalCallback("csDebug", (str) => {
                webView.InjectJavascript(string.Format("scroll({0})", str));
            });

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

        private async void LoadWebViewLayout() {
            var fileContent = Xamarin.Forms.DependencyService.Get<IAssetsManager>();
            webView.Source = await fileContent.GetFileContentAsync("layout.html");
        }

        private void WebView_OnContentLoaded(Xam.Plugin.Abstractions.Events.Inbound.ContentLoadedDelegate eventObj) {
            eventObj.Sender.InjectJavascript("init()");
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
