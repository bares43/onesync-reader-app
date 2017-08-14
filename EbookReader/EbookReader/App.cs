using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookReader.DependencyService;
using EbookReader.Helpers;
using EbookReader.Service;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        FormsWebView webView;
        WebViewMessages _messages;

        public App() {

            webView = new FormsWebView() {
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData,
                WidthRequest = 500,
                HeightRequest = 500
            };

            _messages = new WebViewMessages(webView);

            _messages.OnMyMessage += _messages_OnMyMessage;

            var loadButton = new Button {
                WidthRequest = 150,
                HeightRequest = 50,
                Text = "Načíst knihu"
            };

            loadButton.Clicked += LoadButton_Clicked;

            var goToStartOfPageInput = new Entry {
                WidthRequest = 150,
                HeightRequest = 50
            };

            goToStartOfPageInput.TextChanged += GoToStartOfPageInput_TextChanged;

            this.LoadWebViewLayout();

            webView.OnContentLoaded += WebView_OnContentLoaded;
            
            var content = new ContentPage {
                Title = "EbookReader",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        loadButton,
                        goToStartOfPageInput,
                        webView
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        private void GoToStartOfPageInput_TextChanged(object sender, TextChangedEventArgs e) {
            var value = e.NewTextValue;
            int page;
            if(int.TryParse(value, out page)) {
                var json = new {
                    Page = page
                };

                _messages.Send("goToStartOfPage", json);
            }
        }

        private void _messages_OnMyMessage(object sender, Model.WebViewMessages.MyMessage param) {
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
            var json = new {
                Html = html
            };

            _messages.Send("loadHtml", json);
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
