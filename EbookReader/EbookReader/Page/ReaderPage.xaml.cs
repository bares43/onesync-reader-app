using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Helpers;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Format;
using EbookReader.Model.Messages;
using EbookReader.Page.Reader;
using EbookReader.Provider;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderPage : ContentPage {

        IAssetsManager _assetsManager;
        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;
        ISyncService _syncService;
        IBookmarkService _bookmarkService;

        int currentChapter;

        Book _bookshelfBook;
        Ebook _ebook;

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
            _assetsManager = IocManager.Container.Resolve<IAssetsManager>();
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
            _syncService = IocManager.Container.Resolve<ISyncService>();
            _bookmarkService = IocManager.Container.Resolve<IBookmarkService>();

            // webview events
            WebView.Messages.OnNextChapterRequest += _messages_OnNextChapterRequest;
            WebView.Messages.OnPrevChapterRequest += _messages_OnPrevChapterRequest;
            WebView.Messages.OnOpenQuickPanelRequest += _messages_OnOpenQuickPanelRequest;
            WebView.Messages.OnPageChange += Messages_OnPageChange;
            WebView.Messages.OnChapterRequest += Messages_OnChapterRequest;
            WebView.Messages.OnOpenUrl += Messages_OnOpenUrl;
            WebView.Messages.OnPanEvent += Messages_OnPanEvent;

            QuickPanel.PanelContent.OnChapterChange += PanelContent_OnChapterChange;

            var quickPanelPosition = new Rectangle(0, 0, 1, 0.75);

            if (Device.RuntimePlatform == Device.UWP) {
                quickPanelPosition = new Rectangle(0, 0, 0.33, 1);
            }

            _messageBus.Send(new FullscreenRequestMessage(true));
            
            if (UserSettings.Reader.NightMode) {
                BackgroundColor = Color.FromRgb(24, 24, 25);
            }

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void SubscribeMessages() {
            _messageBus.Subscribe<ChangeMarginMessage>(ChangeMargin, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<ChangeFontSizeMessage>(ChangeFontSize, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<AppSleepMessage>(AppSleepSubscriber, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<AddBookmarkMessage>(AddBookmark, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<OpenBookmarkMessage>(OpenBookmark, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<DeleteBookmarkMessage>(DeleteBookmark, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<ChangedBookmarkNameMessage>(ChangedBookmarkName, new string[] { nameof(ReaderPage) });
            _messageBus.Subscribe<GoToPageMessage>(GoToPageHandler, new string[] { nameof(ReaderPage) });
        }

        private void UnSubscribeMessages() {
            _messageBus.UnSubscribe(nameof(ReaderPage));
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

        private void Messages_OnPanEvent(object sender, Model.WebViewMessages.PanEvent e) {

            if (UserSettings.Control.BrightnessChange == BrightnessChange.None) {
                return;
            }

            var totalWidth = (int)WebView.Width + (2 * UserSettings.Reader.Margin);
            var edge = totalWidth / 5;

            if ((UserSettings.Control.BrightnessChange == BrightnessChange.Left && e.X <= edge) ||
                (UserSettings.Control.BrightnessChange == BrightnessChange.Right && e.X >= totalWidth - edge)) {
                float brightness = 1 - ((float)e.Y / ((int)WebView.Height + (2 * UserSettings.Reader.Margin)));
                _messageBus.Send(new ChangesBrightnessMessage { Brightness = brightness });
            }
        }

        private void Messages_OnChapterRequest(object sender, Model.WebViewMessages.ChapterRequest e) {
            if (!string.IsNullOrEmpty(e.Chapter)) {
                var filename = e.Chapter.Split('#');
                var hash = filename.Skip(1).FirstOrDefault();
                var path = filename.FirstOrDefault();

                var currentChapterPath = _ebook.Files.First(o => o.Id == _ebook.Spines.ElementAt(currentChapter).Idref).Href;

                var newChapterPath = PathHelper.NormalizePath(PathHelper.CombinePath(currentChapterPath, path));
                var chapterId = _ebook.Files.Where(o => PathHelper.NormalizePath(o.Href) == newChapterPath).Select(o => o.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(chapterId)) {
                    var chapter = _ebook.Spines.FirstOrDefault(o => o.Idref == chapterId);

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
            _messageBus.Send(new FullscreenRequestMessage(false));
            this.UnSubscribeMessages();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            backgroundSync = true;
            _messageBus.Send(new FullscreenRequestMessage(true));
            this.SubscribeMessages();

            var task = Task.Run(() => {
                this.LoadProgress();
                this.SynchronizeBookmarks();
            });

            Device.StartTimer(new TimeSpan(0, 1, 0), () => {
                if (backgroundSync) {
                    this.LoadProgress();
                    this.SaveProgress();
                    this.SynchronizeBookmarks();
                }

                return backgroundSync;
            });
        }

        public async void LoadBook(Book book) {
            _bookshelfBook = book;
            _ebook = await EbookFormatHelper.GetBookLoader(book.Format).OpenBook(book.Path);
            var position = _bookshelfBook.Position;

            QuickPanel.PanelContent.SetNavigation(_ebook.Navigation);
            this.RefreshBookmarks();

            var chapter = _ebook.Spines.First();
            var positionInChapter = 0;

            if (position != null) {
                var loadedChapter = _ebook.Spines.ElementAt(position.Spine);
                if (loadedChapter != null) {
                    chapter = loadedChapter;
                    positionInChapter = position.SpinePosition;
                }
            }

            this.SendChapter(chapter, position: positionInChapter);
        }

        private void AppSleepSubscriber(AppSleepMessage msg) {
            if (Device.RuntimePlatform == Device.UWP && backgroundSync) {
                this.SaveProgress();
            }
        }

        private void AddBookmark(AddBookmarkMessage msg) {
            _bookmarkService.CreateBookmark(DateTime.Now.ToString(), _bookshelfBook.ID, _bookshelfBook.Position);

            this.RefreshBookmarks();
        }

        private void DeleteBookmark(DeleteBookmarkMessage msg) {
            _bookmarkService.DeleteBookmark(msg.Bookmark, _bookshelfBook.ID);

            this.RefreshBookmarks();
        }

        public void ChangedBookmarkName(ChangedBookmarkNameMessage msg) {
            _bookmarkService.SaveBookmark(msg.Bookmark);
            _syncService.SaveBookmark(_bookshelfBook.ID, msg.Bookmark);
            this.RefreshBookmarks();
        }

        private async void RefreshBookmarks() {
            var bookmarks = await _bookmarkService.LoadBookmarksByBookID(_bookshelfBook.ID);
            QuickPanel.PanelBookmarks.SetBookmarks(bookmarks);
        }

        private void OpenBookmark(OpenBookmarkMessage msg) {
            var loadedChapter = _ebook.Spines.ElementAt(msg.Bookmark.Position.Spine);
            if (loadedChapter != null) {
                if (currentChapter != msg.Bookmark.Position.Spine) {
                    this.SendChapter(loadedChapter, position: msg.Bookmark.Position.SpinePosition);
                } else {
                    _bookshelfBook.SpinePosition = msg.Bookmark.Position.SpinePosition;
                    this.GoToPosition(msg.Bookmark.Position.SpinePosition);
                }
            }
        }

        private void ChangeMargin(ChangeMarginMessage msg) {
            this.SetMargin(msg.Margin);
        }

        private void ChangeFontSize(ChangeFontSizeMessage msg) {
            this.SetFontSize(msg.FontSize);
        }

        private void GoToPageHandler(GoToPageMessage msg) {
            this.SendGoToPage(msg.Page, msg.Next, msg.Previous);
        }

        private async void SendChapter(Spine chapter, int position = 0, bool lastPage = false, string marker = "") {
            currentChapter = _ebook.Spines.IndexOf(chapter);
            _bookshelfBook.Spine = currentChapter;

            var bookLoader = EbookFormatHelper.GetBookLoader(_bookshelfBook.Format);

            var html = await bookLoader.GetChapter(_ebook, chapter);
            var htmlResult = await bookLoader.PrepareHTML(html, _ebook, _ebook.Files.Where(o => o.Id == chapter.Idref).First());

            Device.BeginInvokeOnMainThread(() => {
                this.SendHtml(htmlResult, position, lastPage, marker);
            });

        }

        #region sync
        private void SaveProgress() {
            if (_bookshelfBook == null) return;
            _bookshelfService.SaveBook(_bookshelfBook);
            if (!_bookshelfBook.Position.Equals(lastSavedPosition)) {
                lastSavedPosition = new Position(_bookshelfBook.Position);
                _syncService.SaveProgress(_bookshelfBook.ID, _bookshelfBook.Position);
            }
        }

        private async void LoadProgress() {
            if (_bookshelfBook == null) return;
            var syncPosition = await _syncService.LoadProgress(_bookshelfBook.ID);
            if (!syncPending &&
                syncPosition != null &&
                syncPosition.Position != null &&
                !syncPosition.Position.Equals(_bookshelfBook.Position) &&
                !syncPosition.Position.Equals(lastLoadedPosition) &&
                syncPosition.D != UserSettings.Synchronization.DeviceName) {
                var loadedChapter = _ebook.Spines.ElementAt(syncPosition.Position.Spine);
                if (loadedChapter != null) {
                    lastLoadedPosition = new Position(syncPosition.Position);
                    Device.BeginInvokeOnMainThread(async () => {
                        syncPending = true;
                        var loadPosition = await DisplayAlert("Reading position at the another device", $"Load reading position from the device {syncPosition.D}?", "Yes, load it", "No");
                        if (loadPosition) {
                            if (currentChapter != syncPosition.Position.Spine) {
                                this.SendChapter(loadedChapter, position: syncPosition.Position.SpinePosition);
                            } else {
                                _bookshelfBook.SpinePosition = syncPosition.Position.SpinePosition;
                                _bookshelfService.SaveBook(_bookshelfBook);
                                this.GoToPosition(syncPosition.Position.SpinePosition);
                            }
                        }
                        syncPending = false;
                    });
                }
            }
        }

        private void SynchronizeBookmarks() {
            if (_bookshelfBook == null) return;
            _syncService.SynchronizeBookmarks(_bookshelfBook);
        }
        #endregion

        #region events
        private void PanelContent_OnChapterChange(object sender, Model.Navigation.Item e) {
            if (e.Id != null) {
                var path = e.Id.Split('#');
                var id = path.First();
                var marker = path.Skip(1).FirstOrDefault() ?? string.Empty;

                var normalizedId = PathHelper.NormalizePath(PathHelper.CombinePath(_ebook.ContentBasePath, id));

                var file = _ebook.Files.FirstOrDefault(o => o.Href.Contains(id) || o.Href.Contains(normalizedId));
                if (file != null) {
                    var spine = _ebook.Spines.FirstOrDefault(o => o.Idref == file.Id);
                    if (spine != null) {
                        //TODO[bares]: pokud se nemeni kapitola, poslat jen marker
                        this.SendChapter(spine, marker: marker);
                    }
                }
            }
        }

        private void Messages_OnPageChange(object sender, Model.WebViewMessages.PageChange e) {
            _bookshelfBook.SpinePosition = e.Position;
            _bookshelfService.SaveBook(_bookshelfBook);
            _messageBus.Send(new PageChangeMessage { CurrentPage = e.CurrentPage, TotalPages = e.TotalPages, Position = e.Position });
        }

        private void _messages_OnOpenQuickPanelRequest(object sender, Model.WebViewMessages.OpenQuickPanelRequest e) {
            QuickPanel.Show();
        }

        private void _messages_OnPrevChapterRequest(object sender, Model.WebViewMessages.PrevChapterRequest e) {
            if (currentChapter > 0) {
                this.SendChapter(_ebook.Spines[currentChapter - 1], lastPage: true);
            }
        }

        private void _messages_OnNextChapterRequest(object sender, Model.WebViewMessages.NextChapterRequest e) {
            if (currentChapter < _ebook.Spines.Count - 1) {
                this.SendChapter(_ebook.Spines[currentChapter + 1]);
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

        private void SendGoToPage(int page, bool next, bool previous) {
            var json = new {
                Page = page,
                Next = next,
                Previous = previous,
            };

            WebView.Messages.Send("goToPage", json);
        }
        #endregion
    }
}