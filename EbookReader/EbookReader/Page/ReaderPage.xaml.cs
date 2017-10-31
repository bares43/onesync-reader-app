using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Service;
using HtmlAgilityPack;
using Plugin.FilePicker.Abstractions;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderPage : ContentPage {

        IWebViewMessages _messages;
        IEpubLoader _epubLoader;
        IAssetsManager _assetsManager;

        Model.EpubSpine currentChapter;

        Model.Epub epub;

        public ReaderPage() {
            InitializeComponent();

            InitPage();
        }

        public void InitPage() {
            
            // ioc
            _messages = IocManager.Container.Resolve<IWebViewMessages>();
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();
            _assetsManager = IocManager.Container.Resolve<IAssetsManager>();
            
            // webview events
            _messages.OnNextChapterRequest += _messages_OnNextChapterRequest;
            _messages.OnPrevChapterRequest += _messages_OnPrevChapterRequest;
            _messages.OnOpenQuickPanelRequest += _messages_OnOpenQuickPanelRequest;
            
            this.LoadWebViewLayout();

            QuickPanel.PanelContent.OnChapterChange += PanelContent_OnChapterChange;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public static FormsWebView Factory() {
            return IocManager.Container.Resolve<FormsWebView>();
        }

        private void PanelContent_OnChapterChange(object sender, Model.Navigation.Item e) {
            var file = epub.Files.FirstOrDefault(o => o.Href == e.Id);
            if (file != null) {
                var spine = epub.Spines.FirstOrDefault(o => o.Idref == file.Id);
                if (spine != null) {
                    this.SendChapter(spine);
                }
            }
        }

        private void _messages_OnOpenQuickPanelRequest(object sender, Model.WebViewMessages.OpenQuickPanelRequest e) {
            QuickPanel.Show();
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.SettingsPage());
        }

        private async void HomeButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.HomePage());
        }

        public async Task LoadBook(FileData file) {
            epub = await _epubLoader.GetEpub(file.FileName, file.DataArray);
            QuickPanel.PanelContent.SetNavigation(epub.Navigation);

            this.SendChapter(epub.Spines.First());
        }

        private async void LoadWebViewLayout() {
            var layout = await _assetsManager.GetFileContentAsync("layout.html");
            var js = await _assetsManager.GetFileContentAsync("reader.js");
            var css = await _assetsManager.GetFileContentAsync("reader.css");

            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode(string.Format("<script>{0}</script>", js)));
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode(string.Format("<style>{0}</style>", css)));

            WebView.Source = doc.DocumentNode.OuterHtml;
        }
        
        private void _messages_OnPrevChapterRequest(object sender, Model.WebViewMessages.PrevChapterRequest e) {
            var i = epub.Spines.IndexOf(currentChapter);
            if (i > 0) {
                this.SendChapter(epub.Spines[i - 1], "last");
            }
        }

        private void _messages_OnNextChapterRequest(object sender, Model.WebViewMessages.NextChapterRequest e) {
            var i = epub.Spines.IndexOf(currentChapter);
            if (i < epub.Spines.Count - 1) {
                this.SendChapter(epub.Spines[i + 1]);
            }
        }

        private void WebView_OnContentLoaded(object sender, EventArgs e) {
            this.InitWebView(
                (int)WebView.Width,
                (int)WebView.Height,
                30,
                20
            );
        }

        private async void SendChapter(Model.EpubSpine chapter, string page = "") {
            currentChapter = chapter;

            var html = await _epubLoader.GetChapter(epub, chapter);
            var htmlResult = await _epubLoader.PrepareHTML(html, epub.Folder);

            Device.BeginInvokeOnMainThread(() => {
                this.SendHtml(htmlResult, page);
            });

        }

        private void WebView_SizeChanged(object sender, EventArgs e) {
            this.ResizeWebView((int)WebView.Width, (int)WebView.Height);
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