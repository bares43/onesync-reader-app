using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookReader.DependencyService;
using EbookReader.Service;
using Plugin.FilePicker;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        FormsWebView webView;
        WebViewMessages _messages;
        Picker fontSizePicker;
        Picker marginPicker;
        Label pages;

        List<string> FontSizes {
            get {
                return new List<string> {
                    "12",
                    "14",
                    "16",
                    "18",
                    "20",
                    "22",
                    "24",
                    "26",
                    "28",
                    "30",
                    "32",
                    "34",
                    "36",
                    "38",
                    "40"
                };
            }
        }

        List<string> Margins {
            get {
                return new List<string> {
                    "15",
                    "30",
                    "45",
                };
            }
        }

        public App() {

            this.pages = new Label();

            webView = new FormsWebView() {
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _messages = new WebViewMessages(webView);
            _messages.OnPageChange += _messages_OnPageChange;

            var loadButton = new Button {
                Text = "Načíst knihu"
            };

            loadButton.Clicked += LoadButton_Clicked;

            var goToStartOfPageInput = new Entry();

            goToStartOfPageInput.TextChanged += GoToStartOfPageInput_TextChanged;

            fontSizePicker = new Picker {
                Title = "Velikost písma",
                ItemsSource = this.FontSizes
            };

            fontSizePicker.SelectedIndexChanged += FontSizePicker_SelectedIndexChanged;

            marginPicker = new Picker {
                Title = "Velikost odsazení",
                ItemsSource = this.Margins
            };

            marginPicker.SelectedIndexChanged += MarginPicker_SelectedIndexChanged;

            this.LoadWebViewLayout();

            webView.OnContentLoaded += WebView_OnContentLoaded;
            webView.SizeChanged += WebView_SizeChanged;

            var controls = new StackLayout {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = {
                    loadButton,
                    new StackLayout {
                        Children = {
                            pages,
                            goToStartOfPageInput,
                        }
                    },
                    fontSizePicker,
                    marginPicker,
                }
            };

            var content = new ContentPage {
                Title = "EbookReader",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children = {
                        controls,
                        webView,
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        private void _messages_OnPageChange(object sender, Model.WebViewMessages.PageChange e) {
            this.pages.Text = string.Format("Stránka {0} z {1}", e.CurrentPage, e.TotalPages);
        }

        private void MarginPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.marginPicker.SelectedIndex != -1) {
                var margin = int.Parse(this.Margins[this.marginPicker.SelectedIndex]);
                this.SetMargin(margin);
            }
        }

        private void WebView_SizeChanged(object sender, EventArgs e) {
            this.ResizeWebView((int)this.webView.Width, (int)this.webView.Height);
        }

        private void FontSizePicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.fontSizePicker.SelectedIndex != -1) {
                var fontSize = int.Parse(this.FontSizes[this.fontSizePicker.SelectedIndex]);
                this.SetFontSize(fontSize);
            }
        }


        private void GoToStartOfPageInput_TextChanged(object sender, TextChangedEventArgs e) {
            var value = e.NewTextValue;
            int page;
            if (int.TryParse(value, out page)) {
                var json = new {
                    Page = page
                };

                _messages.Send("goToStartOfPage", json);
            }
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

        private void SetFontSize(int fontSize) {
            var json = new {
                FontSize = fontSize
            };

            _messages.Send("changeFontSize", json);
        }

        private void SetMargin(int margin) {
            var json = new {
                Margin = margin
            };

            _messages.Send("changeMargin", json);
        }

        private void InitWebView(int width, int height) {
            var json = new {
                Width = width,
                Height = height
            };

            _messages.Send("init", json);
        }

        private void ResizeWebView(int width, int height) {
            var json = new {
                Width = width,
                Height = height
            };

            _messages.Send("resize", json);
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
            this.InitWebView((int)this.webView.Width, (int)this.webView.Height);
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
