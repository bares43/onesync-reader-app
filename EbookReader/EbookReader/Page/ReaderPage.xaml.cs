using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Page.Reader;
using EbookReader.Service;
using HtmlAgilityPack;
using Plugin.FilePicker.Abstractions;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderPage : ContentPage {

        FormsWebView _webView;
        IWebViewMessages _messages;
        IEpubLoader _epubLoader;
        IAssetsManager _assetsManager;

        QuickPanel quickPanel;
        Picker fontSizePicker;
        Picker marginPicker;
        Label pages;
        Picker chaptersPicker;
        int chapterPickerLastIndex = -1;

        Model.Epub epub;

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

        public ReaderPage() {
            InitializeComponent();

            InitPage();
        }

        public void InitPage() {
            // ioc
            _webView = IocManager.Container.Resolve<FormsWebView>();
            _messages = IocManager.Container.Resolve<IWebViewMessages>();
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();
            _assetsManager = IocManager.Container.Resolve<IAssetsManager>();

            // setup webview
            _webView.ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData;
            _webView.VerticalOptions = LayoutOptions.FillAndExpand;
            _webView.HorizontalOptions = LayoutOptions.FillAndExpand;

            // webview events
            _messages.OnPageChange += _messages_OnPageChange;
            _messages.OnNextChapterRequest += _messages_OnNextChapterRequest;
            _messages.OnPrevChapterRequest += _messages_OnPrevChapterRequest;
            _messages.OnOpenQuickPanelRequest += _messages_OnOpenQuickPanelRequest;

            _webView.OnContentLoaded += WebView_OnContentLoaded;
            _webView.SizeChanged += WebView_SizeChanged;


            this.pages = new Label() {
                WidthRequest = 75,
            };

            var goToStartOfPageInput = new Entry();

            goToStartOfPageInput.TextChanged += GoToStartOfPageInput_TextChanged;

            fontSizePicker = new Picker {
                Title = "Písmo",
                ItemsSource = this.FontSizes,
                SelectedItem = "20",
            };

            fontSizePicker.SelectedIndexChanged += FontSizePicker_SelectedIndexChanged;

            marginPicker = new Picker {
                Title = "Odsazení",
                ItemsSource = this.Margins,
                SelectedItem = "30",
            };

            marginPicker.SelectedIndexChanged += MarginPicker_SelectedIndexChanged;

            chaptersPicker = new Picker {
                Title = "Kapitola",
                IsVisible = false,
            };

            chaptersPicker.SelectedIndexChanged += ChaptersPicker_SelectedIndexChanged;

            this.LoadWebViewLayout();

            var homeButton = new Button {
                Text = "Domů"
            };
            homeButton.Clicked += HomeButton_Clicked;

            var settingsButton = new Button {
                Text = "Nastavení"
            };
            settingsButton.Clicked += SettingsButton_Clicked;

            //var controls = new StackLayout {
            //    VerticalOptions = LayoutOptions.Start,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    Orientation = StackOrientation.Horizontal,
            //    Children = {
            //        homeButton,
            //        settingsButton,
            //        new StackLayout {
            //            Children = {
            //                pages,
            //                goToStartOfPageInput,
            //            }
            //        },
            //        fontSizePicker,
            //        marginPicker,
            //        chaptersPicker,
            //    }
            //};

            quickPanel = new QuickPanel();

            var quickPanelPosition = new Rectangle(0, 0, 1, 0.75);

            if (Device.RuntimePlatform == Device.Windows) {
                quickPanelPosition = new Rectangle(0, 0, 0.33, 1);
            }

            AbsoluteLayout.SetLayoutBounds(quickPanel, quickPanelPosition);
            AbsoluteLayout.SetLayoutFlags(quickPanel, AbsoluteLayoutFlags.SizeProportional);

            AbsoluteLayout.SetLayoutBounds(_webView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_webView, AbsoluteLayoutFlags.SizeProportional);

            Content = new AbsoluteLayout {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    //controls,
                    _webView,
                    quickPanel,
                }
            };

            NavigationPage.SetHasNavigationBar(this, false);

        }

        private void _messages_OnOpenQuickPanelRequest(object sender, Model.WebViewMessages.OpenQuickPanelRequest e) {
            quickPanel.Show();
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.SettingsPage());
        }

        private async void HomeButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.HomePage());
        }

        public async Task LoadBook(FileData file) {
            epub = await _epubLoader.GetEpub(file.FileName, file.DataArray);

            this.chaptersPicker.ItemsSource = epub.Spines.Select(o => o.Idref).ToList();
            if (this.chaptersPicker.ItemsSource.Count > 0) {
                this.chaptersPicker.SelectedIndex = 0;
            }
            this.chaptersPicker.IsVisible = true;

            this.SendChapter(0);
        }

        private async void LoadWebViewLayout() {
            var layout = await _assetsManager.GetFileContentAsync("layout.html");
            var js = await _assetsManager.GetFileContentAsync("reader.js");
            var css = await _assetsManager.GetFileContentAsync("reader.css");

            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode(string.Format("<script>{0}</script>", js)));
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode(string.Format("<style>{0}</style>", css)));

            _webView.Source = doc.DocumentNode.OuterHtml;
        }

        private void _messages_OnPageChange(object sender, Model.WebViewMessages.PageChange e) {
            Device.BeginInvokeOnMainThread(() => {
                this.pages.Text = string.Format("{0} / {1}", e.CurrentPage, e.TotalPages);
            });
        }

        private void _messages_OnPrevChapterRequest(object sender, Model.WebViewMessages.PrevChapterRequest e) {
            Device.BeginInvokeOnMainThread(() => {
                var index = this.chaptersPicker.SelectedIndex - 1;
                if (index > 0) {
                    this.chaptersPicker.SelectedIndex = index;
                }
            });
        }

        private void _messages_OnNextChapterRequest(object sender, Model.WebViewMessages.NextChapterRequest e) {
            Device.BeginInvokeOnMainThread(() => {
                var index = this.chaptersPicker.SelectedIndex + 1;
                if (this.chaptersPicker.ItemsSource != null && index < this.chaptersPicker.ItemsSource.Count) {
                    this.chaptersPicker.SelectedIndex = index;
                }
            });
        }

        private void WebView_OnContentLoaded(Xam.Plugin.Abstractions.Events.Inbound.ContentLoadedDelegate eventObj) {
            this.InitWebView(
                (int)this._webView.Width,
                (int)this._webView.Height,
                int.Parse((string)this.marginPicker.SelectedItem),
                int.Parse((string)this.fontSizePicker.SelectedItem)
            );
        }


        private async void SendChapter(int chapter, string page = "") {
            var html = await _epubLoader.GetChapter(epub, epub.Spines.Skip(chapter).First());
            var htmlResult = await _epubLoader.PrepareHTML(html, epub.Folder);
            this.SendHtml(htmlResult, page);
        }

        private void ChaptersPicker_SelectedIndexChanged(object sender, EventArgs e) {
            var index = this.chaptersPicker.SelectedIndex;
            if (this.epub != null && index != -1 && index != this.chapterPickerLastIndex) {
                this.SendChapter(index, index < this.chapterPickerLastIndex ? "last" : "");
                this.chapterPickerLastIndex = index;
            }
        }

        private void MarginPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.marginPicker.SelectedIndex != -1) {
                var margin = int.Parse(this.Margins[this.marginPicker.SelectedIndex]);
                this.SetMargin(margin);
            }
        }

        private void WebView_SizeChanged(object sender, EventArgs e) {
            this.ResizeWebView((int)this._webView.Width, (int)this._webView.Height);
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

        private void InitWebView(int width, int height, int margin, int fontSize) {
            var json = new {
                Width = width,
                Height = height,
                Margin = margin,
                FontSize = fontSize,
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

        private void SendHtml(Model.EpubLoader.HtmlResult htmlResult, string page) {
            var json = new {
                Html = htmlResult.Html,
                Images = htmlResult.Images,
                Page = page,
            };

            _messages.Send("loadHtml", json);
        }
    }
}