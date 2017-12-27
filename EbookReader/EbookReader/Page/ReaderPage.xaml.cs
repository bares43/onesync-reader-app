using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Helpers;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Messages;
using EbookReader.Page.Reader;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderPage : ContentPage {

        IEpubLoader _epubLoader;
        IAssetsManager _assetsManager;
        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;
        ISyncService _syncService;

        int currentChapter;

        Book _book;
        Model.Epub _epub;

        bool ResizeFirstRun = true;
        bool ResizeTimerRunning = false;
        int? ResizeTimerWidth;
        int? ResizeTimerHeight;

        bool backgroundSync = true;
        Position lastSavedPosition = null;
        Position lastLoadedPosition = new Position();
        bool syncPending = false;

        public ReaderPage() {
            InitializeComponent();

            // ioc
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();
            _assetsManager = IocManager.Container.Resolve<IAssetsManager>();
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
            _syncService = IocManager.Container.Resolve<ISyncService>();

            // webview events
            WebView.Messages.OnNextChapterRequest += _messages_OnNextChapterRequest;
            WebView.Messages.OnPrevChapterRequest += _messages_OnPrevChapterRequest;
            WebView.Messages.OnOpenQuickPanelRequest += _messages_OnOpenQuickPanelRequest;
            WebView.Messages.OnPageChange += Messages_OnPageChange;
            WebView.Messages.OnChapterRequest += Messages_OnChapterRequest;
            WebView.Messages.OnOpenUrl += Messages_OnOpenUrl;

            QuickPanel.PanelContent.OnChapterChange += PanelContent_OnChapterChange;

            var quickPanelPosition = new Rectangle(0, 0, 1, 0.75);

            if (Device.RuntimePlatform == Device.UWP) {
                quickPanelPosition = new Rectangle(0, 0, 0.33, 1);
            }

            _messageBus.Subscribe<ChangeMargin>((msg) => this.SetMargin(msg.Margin));
            _messageBus.Subscribe<ChangeFontSize>((msg) => this.SetFontSize(msg.FontSize));
            _messageBus.Subscribe<AppSleep>(AppSleepSubscriber);
            _messageBus.Subscribe<AddBookmark>(AddBookmark);
            _messageBus.Subscribe<OpenBookmark>(OpenBookmark);
            _messageBus.Subscribe<DeleteBookmark>(DeleteBookmark);
            _messageBus.Subscribe<ChangedBookmarkName>(ChangedBookmarkName);

            Device.StartTimer(new TimeSpan(0, 5, 0), () => {
                if (backgroundSync) {
                    this.LoadProgress();
                    this.SaveProgress();
                }

                return backgroundSync;
            });

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed() {
            if (QuickPanel.IsVisible) {
                QuickPanel.Hide();
                return true;
            }

            return base.OnBackButtonPressed();
        }

        private void Messages_OnOpenUrl(object sender, Model.WebViewMessages.OpenUrl e) {
            if (!string.IsNullOrEmpty(e.Url)) {
                try {
                    var uri = new Uri(e.Url);
                    Device.OpenUri(uri);
                } catch (Exception) { }
            }
        }

        private void Messages_OnChapterRequest(object sender, Model.WebViewMessages.ChapterRequest e) {
            if (!string.IsNullOrEmpty(e.Chapter)) {
                var filename = e.Chapter.Split('#');
                var hash = filename.Skip(1).FirstOrDefault();
                var path = filename.FirstOrDefault();

                var currentChapterPath = _epub.Files.First(o => o.Id == _epub.Spines.ElementAt(currentChapter).Idref).Href;

                var newChapterPath = PathHelper.NormalizePath(PathHelper.CombinePath(currentChapterPath, path));
                var chapterId = _epub.Files.Where(o => PathHelper.NormalizePath(o.Href) == newChapterPath).Select(o => o.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(chapterId)) {
                    var chapter = _epub.Spines.FirstOrDefault(o => o.Idref == chapterId);

                    System.Diagnostics.Debug.WriteLine(chapter);
                    if (chapter != null) {
                        this.SendChapter(chapter, marker: hash);
                    }
                }

            }
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            this.SaveProgress();
            backgroundSync = false;
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            backgroundSync = true;
        }

        public async void LoadBook(Book book) {
            _book = book;
            _epub = await _epubLoader.OpenEpub(book.Path);
            var position = _book.Position;

            QuickPanel.PanelContent.SetNavigation(_epub.Navigation);
            QuickPanel.PanelBookmarks.SetBookmarks(_book.Bookmarks);

            var chapter = _epub.Spines.First();
            var positionInChapter = 0;

            if (position != null) {
                var loadedChapter = _epub.Spines.ElementAt(position.Spine);
                if (loadedChapter != null) {
                    chapter = loadedChapter;
                    positionInChapter = position.SpinePosition;
                }
            }

            this.SendChapter(chapter, position: positionInChapter);

            var task = Task.Run(() => {
                this.LoadProgress();
            });
        }

        private void AppSleepSubscriber(AppSleep msg) {
            if (Device.RuntimePlatform == Device.UWP && backgroundSync) {
                this.SaveProgress();
            }
        }

        private void AddBookmark(AddBookmark msg) {
            _book.Bookmarks.Add(new Bookmark {
                Name = DateTime.Now.ToString(),
                Position = new Position(_book.Position)
            });
            _bookshelfService.SaveBook(_book);
            QuickPanel.PanelBookmarks.SetBookmarks(_book.Bookmarks);
        }

        private void DeleteBookmark(DeleteBookmark msg) {
            _book.Bookmarks.Remove(msg.Bookmark);
            _bookshelfService.SaveBook(_book);
            QuickPanel.PanelBookmarks.SetBookmarks(_book.Bookmarks);
        }

        public void ChangedBookmarkName(ChangedBookmarkName msg) {
            _bookshelfService.SaveBook(_book);
            QuickPanel.PanelBookmarks.SetBookmarks(_book.Bookmarks);
        }

        private void OpenBookmark(OpenBookmark msg) {
            var loadedChapter = _epub.Spines.ElementAt(msg.Bookmark.Position.Spine);
            if (loadedChapter != null) {
                if (currentChapter != msg.Bookmark.Position.Spine) {
                    this.SendChapter(loadedChapter, position: msg.Bookmark.Position.SpinePosition);
                } else {
                    _book.Position.SpinePosition = msg.Bookmark.Position.SpinePosition;
                    this.GoToPosition(msg.Bookmark.Position.SpinePosition);
                }
            }
        }

        private async void SendChapter(Model.EpubSpine chapter, int position = 0, bool lastPage = false, string marker = "") {
            currentChapter = _epub.Spines.IndexOf(chapter);
            _book.Position.Spine = currentChapter;

            var html = await _epubLoader.GetChapter(_epub, chapter);
            var htmlResult = await _epubLoader.PrepareHTML(html, _epub);

            Device.BeginInvokeOnMainThread(() => {
                this.SendHtml(htmlResult, position, lastPage, marker);
            });

        }

        #region sync
        private void SaveProgress() {
            _bookshelfService.SaveBook(_book);
            if (!_book.Position.Equals(lastSavedPosition)) {
                lastSavedPosition = new Position(_book.Position);
                _syncService.SaveProgress(_book.Id, _book.Position);
            }
        }

        private async void LoadProgress() {
            var syncPosition = await _syncService.LoadProgress(_book.Id);
            if (!syncPending &&
                syncPosition != null &&
                syncPosition.Position != null &&
                !syncPosition.Position.Equals(_book.Position) &&
                !syncPosition.Position.Equals(lastLoadedPosition) &&
                syncPosition.D != UserSettings.Synchronization.DeviceName) {
                var loadedChapter = _epub.Spines.ElementAt(syncPosition.Position.Spine);
                if (loadedChapter != null) {
                    lastLoadedPosition = new Position(syncPosition.Position);
                    Device.BeginInvokeOnMainThread(async () => {
                        syncPending = true;
                        var loadPosition = await DisplayAlert("Postup čtení na jiném zařízení", $"Načíst postup čtení ze zařízení {syncPosition.D}?", "Načíst", "Ne");
                        if (loadPosition) {
                            if (currentChapter != syncPosition.Position.Spine) {
                                this.SendChapter(loadedChapter, position: syncPosition.Position.SpinePosition);
                            } else {
                                _book.Position.SpinePosition = syncPosition.Position.SpinePosition;
                                this.GoToPosition(syncPosition.Position.SpinePosition);
                            }
                        }
                        syncPending = false;
                    });
                }
            }
        }
        #endregion

        #region events
        private void PanelContent_OnChapterChange(object sender, Model.Navigation.Item e) {
            if (e.Id != null) {
                var path = e.Id.Split('#');
                var id = path.First();
                var marker = path.Skip(1).FirstOrDefault() ?? string.Empty;

                var normalizedId = PathHelper.NormalizePath(PathHelper.CombinePath(_epub.ContentBasePath, id));

                var file = _epub.Files.FirstOrDefault(o => o.Href.Contains(id) || o.Href.Contains(normalizedId));
                if (file != null) {
                    var spine = _epub.Spines.FirstOrDefault(o => o.Idref == file.Id);
                    if (spine != null) {
                        //TODO[bares]: pokud se nemeni kapitola, poslat jen marker
                        this.SendChapter(spine, marker: marker);
                    }
                }
            }
        }

        private void Messages_OnPageChange(object sender, Model.WebViewMessages.PageChange e) {
            _book.Position.SpinePosition = e.Position;
        }

        private void _messages_OnOpenQuickPanelRequest(object sender, Model.WebViewMessages.OpenQuickPanelRequest e) {
            QuickPanel.Show();
        }

        private void _messages_OnPrevChapterRequest(object sender, Model.WebViewMessages.PrevChapterRequest e) {
            if (currentChapter > 0) {
                this.SendChapter(_epub.Spines[currentChapter - 1], lastPage: true);
            }
        }

        private void _messages_OnNextChapterRequest(object sender, Model.WebViewMessages.NextChapterRequest e) {
            if (currentChapter < _epub.Spines.Count - 1) {
                this.SendChapter(_epub.Spines[currentChapter + 1]);
            }
        }

        private void WebView_OnContentLoaded(object sender, EventArgs e) {
            this.InitWebView(
                (int)WebView.Width,
                (int)WebView.Height
            );
        }

        private void WebView_SizeChanged(object sender, EventArgs e) {

            if (ResizeFirstRun) {
                ResizeFirstRun = false;
                return;
            }

            ResizeTimerWidth = (int)WebView.Width;
            ResizeTimerHeight = (int)WebView.Height;

            if (!ResizeTimerRunning) {
                ResizeTimerRunning = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), () => {

                    if (ResizeTimerWidth.HasValue && ResizeTimerHeight.HasValue) {
                        this.ResizeWebView(ResizeTimerWidth.Value, ResizeTimerHeight.Value);
                    }

                    ResizeTimerRunning = false;

                    return false;
                });
            }
        }
        #endregion

        #region webview messages
        private void InitWebView(int width, int height) {
            var json = new {
                Width = width,
                Height = height,
                Margin = UserSettings.Reader.Margin,
                FontSize = UserSettings.Reader.FontSize,
                ScrollSpeed = UserSettings.Reader.ScrollSpeed,
                ClickEverywhere = UserSettings.Control.ClickEverywhere,
                DoubleSwipe = UserSettings.Control.DoubleSwipe,
                NightMode = UserSettings.Reader.NightMode,
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

        private void SendHtml(Model.EpubLoader.HtmlResult htmlResult, int position = 0, bool lastPage = false, string marker = "") {
            var json = new {
                Html = htmlResult.Html,
                Images = htmlResult.Images,
                Position = position,
                LastPage = lastPage,
                Marker = marker,
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

        private void GoToPosition(int position) {
            var json = new {
                Position = position,
            };

            WebView.Messages.Send("goToPosition", json);
        }
        #endregion
    }
}