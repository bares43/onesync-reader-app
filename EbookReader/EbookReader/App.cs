using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookReader.DependencyService;
using EbookReader.Service;
using Newtonsoft.Json;
using Plugin.FilePicker;
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

            var loadButton = new Button {
                WidthRequest = 500,
                HeightRequest = 50,
                Text = "Načíst knihu"
            };

            loadButton.Clicked += LoadButton_Clicked;

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
                        loadButton,
                        webView
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        private void LoadButton_Clicked(object sender, EventArgs e) {
            this.LoadBook();
        }

        public async void LoadBook() {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            var loader = new EpubLoader();
            var epub = await loader.GetEpub(pickedFile.FileName, pickedFile.DataArray);

            var chapter = await loader.GetChapter(epub, epub.Spines.Skip(7).First());

            var html = loader.PrepareHTML(chapter);
            this.SendHtml(html);
        }

        private void SendHtml(string html) {
            var json = JsonConvert.SerializeObject(new {
                Html = html
            });

            var js = string.Format("loadHtml('{0}')", Base64Encode(json));

            webView.InjectJavascript(js);
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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
