using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Model.Messages;
using EbookReader.Page.Reader;
using EbookReader.Service;
using HtmlAgilityPack;
using Plugin.FilePicker.Abstractions;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderPage : ContentPage {

        IEpubLoader _epubLoader;
        IAssetsManager _assetsManager;
        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;

        Model.EpubSpine currentChapter;

        Model.Epub _epub;

        public ReaderPage() {
            InitializeComponent();

            // ioc
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();
            _assetsManager = IocManager.Container.Resolve<IAssetsManager>();
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            // webview events
            WebView.Messages.OnNextChapterRequest += _messages_OnNextChapterRequest;
            WebView.Messages.OnPrevChapterRequest += _messages_OnPrevChapterRequest;
            WebView.Messages.OnOpenQuickPanelRequest += _messages_OnOpenQuickPanelRequest;

            QuickPanel.PanelContent.OnChapterChange += PanelContent_OnChapterChange;

            var quickPanelPosition = new Rectangle(0, 0, 1, 0.75);

            if (Device.RuntimePlatform == Device.UWP) {
                quickPanelPosition = new Rectangle(0, 0, 0.33, 1);
            }

            _messageBus.Subscribe<ChangeMargin>((msg) => this.SetMargin(msg.Margin));
            _messageBus.Subscribe<ChangeFontSize>((msg) => this.SetFontSize(msg.FontSize));

            NavigationPage.SetHasNavigationBar(this, false);

        }


        private void PanelContent_OnChapterChange(object sender, Model.Navigation.Item e) {
            var file = _epub.Files.FirstOrDefault(o => o.Href == e.Id);
            if (file != null) {
                var spine = _epub.Spines.FirstOrDefault(o => o.Idref == file.Id);
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

        public void LoadBook(Model.Epub epub) {
            _epub = epub;
            QuickPanel.PanelContent.SetNavigation(epub.Navigation);
            this.SendChapter(epub.Spines.First());
        }


        private void _messages_OnPrevChapterRequest(object sender, Model.WebViewMessages.PrevChapterRequest e) {
            var i = _epub.Spines.IndexOf(currentChapter);
            if (i > 0) {
                this.SendChapter(_epub.Spines[i - 1], "last");
            }
        }

        private void _messages_OnNextChapterRequest(object sender, Model.WebViewMessages.NextChapterRequest e) {
            var i = _epub.Spines.IndexOf(currentChapter);
            if (i < _epub.Spines.Count - 1) {
                this.SendChapter(_epub.Spines[i + 1]);
            }
        }

        private void WebView_OnContentLoaded(object sender, EventArgs e) {
            this.InitWebView(
                (int)WebView.Width,
                (int)WebView.Height,
                UserSettings.Reader.Margin,
                UserSettings.Reader.FontSize,
                UserSettings.Reader.ScrollSpeed
            );
        }

        private async void SendChapter(Model.EpubSpine chapter, string page = "") {
            currentChapter = chapter;

            var html = await _epubLoader.GetChapter(_epub, chapter);
            var htmlResult = await _epubLoader.PrepareHTML(html, _epub.Folder);

            Device.BeginInvokeOnMainThread(() => {
                this.SendHtml(htmlResult, page);
            });

        }

        private void WebView_SizeChanged(object sender, EventArgs e) {
            this.ResizeWebView((int)this.WebView.Width, (int)this.WebView.Height);
        }

        private void GoToStartOfPageInput_TextChanged(object sender, TextChangedEventArgs e) {
            var value = e.NewTextValue;
            int page;
            if (int.TryParse(value, out page)) {
                var json = new {
                    Page = page
                };

                WebView.Messages.Send("goToStartOfPage", json);
            }
        }

        private void InitWebView(int width, int height, int margin, int fontSize, int scrollSpeed) {
            var json = new {
                Width = width,
                Height = height,
                Margin = margin,
                FontSize = fontSize,
                ScrollSpeed = scrollSpeed,
            };

            WebView.Messages.Send("init", json);
        }

        private void ResizeWebView(int width, int height) {
            var json = new {
                Width = width,
                Height = height
            };

            WebView.Messages.Send("resize", json);
        }

        private void SendHtml(Model.EpubLoader.HtmlResult htmlResult, string page) {
            var json = new {
                Html = htmlResult.Html,
                Images = htmlResult.Images,
                Page = page,
            };

            WebView.Messages.Send("loadHtml", json);
        }

        private void SetFontSize(int fontSize) {
            var json = new {
                FontSize = fontSize
            };

            WebView.Messages.Send("changeFontSize", json);
        }

        private void SetMargin(int margin) {
            var json = new {
                Margin = margin
            };

            WebView.Messages.Send("changeMargin", json);
        }
    }
}