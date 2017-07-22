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
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData,
                Source = GetHtml(),
                WidthRequest = 500,
                HeightRequest = 500
            };

            webView.OnContentLoaded += WebView_OnContentLoaded;

            webView.RegisterGlobalCallback("csDebug", (str) => {
                str.ToString();
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

        private void WebView_OnContentLoaded(Xam.Plugin.Abstractions.Events.Inbound.ContentLoadedDelegate eventObj) {
            eventObj.Sender.InjectJavascript("csDebug('Internet WebView Loaded Successfully!')");
            eventObj.Sender.InjectJavascript("test('zmena nadpisu z C#')");
        }


        protected string GetHtml() {
            return @"
<html>
    <head>
        <script type='text/javascript' src='https://code.jquery.com/jquery-3.2.1.min.js'></script>
        <script type='text/javascript'>
            function test(str){
               $('h1').text(str);
               csDebug('zmena nadpisu probehla uspesne');
            }
        </script>
    </head>
    <body>
        <h1>nadpis</h1>
    </body>
</html>
";
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
