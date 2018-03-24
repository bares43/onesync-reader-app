using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Messages;
using EbookReader.Model.Sync;
using EbookReader.Provider;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace EbookReader.Service {
    public class SyncService : ISyncService {

        const string ProgressNode = "progress";
        const string BookmarksNode = "bookmarks";
        const string BookmarksLastChangeNode = "bookmarkslastchange";

        ICloudStorageService _cloudStorageService;
        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;

        public SyncService(IBookshelfService bookshelfService, IMessageBus messageBus) {
            _bookshelfService = bookshelfService;
            _messageBus = messageBus;

            var service = UserSettings.Synchronization.Enabled ? UserSettings.Synchronization.Service : SynchronizationServicesProvider.Dumb;
            _cloudStorageService = IocManager.Container.ResolveKeyed<ICloudStorageService>(service);
        }

        public async Task<Progress> LoadProgress(string bookID) {

            if (!CanSync()) return null;

            var path = this.PathGenerator(bookID, ProgressNode);

            return await _cloudStorageService.LoadJson<Progress>(path);
        }

        public void SaveProgress(string bookID, Position position) {

            if (!CanSync()) return;

            var progress = new Progress {
                DeviceName = UserSettings.Synchronization.DeviceName,
                Position = position,
            };

            var path = this.PathGenerator(bookID, ProgressNode);

            _cloudStorageService.SaveJson(progress, path);
        }

        public void DeleteBook(string bookID) {

            if (!CanSync()) return;

            _cloudStorageService.DeleteNode(this.PathGenerator(bookID));
        }

        public void SaveBookmark(string bookID, Model.Bookshelf.Bookmark bookmark) {

            if (!CanSync()) return;

            var syncBookmark = new Model.Sync.Bookmark {
                ID = bookmark.ID,
                Name = bookmark.Name,
                Position = bookmark.Position,
                Deleted = bookmark.Deleted,
                LastChange = bookmark.LastChange,
            };

            var path = this.PathGenerator(bookID, BookmarksNode, bookmark.ID.ToString());

            _cloudStorageService.SaveJson(syncBookmark, path);
            this.SaveBookmarksLastChange(bookID);
        }

        public async void SynchronizeBookmarks(Book book) {

            if (!CanSync()) return;

            var bookmarkService = IocManager.Container.Resolve<IBookmarkService>();

            var data = await _cloudStorageService.LoadJson<DateTime?>(this.PathGenerator(book.ID, BookmarksLastChangeNode));

            if (!data.HasValue || !book.BookmarksSyncLastChange.HasValue || book.BookmarksSyncLastChange.Value < data.Value) {

                var cloudBookmarks = await _cloudStorageService.LoadJsonList<Model.Sync.Bookmark>(this.PathGenerator(book.ID, BookmarksNode));
                var deviceBookmarks = await bookmarkService.LoadBookmarksByBookID(book.ID);

                var change = false;

                foreach (var cloudBookmark in cloudBookmarks) {
                    var deviceBookmark = deviceBookmarks.FirstOrDefault(o => o.ID == cloudBookmark.ID);
                    if (deviceBookmark == null && !cloudBookmark.Deleted) {
                        deviceBookmark = new Model.Bookshelf.Bookmark {
                            ID = cloudBookmark.ID,
                            BookID = book.ID,
                            Name = cloudBookmark.Name,
                            Position = cloudBookmark.Position,
                            LastChange = DateTime.UtcNow,
                        };

                        deviceBookmarks.Add(deviceBookmark);

                        change = true;
                        bookmarkService.SaveBookmark(deviceBookmark);
                    } else if (deviceBookmark != null && deviceBookmark.LastChange < cloudBookmark.LastChange) {
                        deviceBookmark.Name = cloudBookmark.Name;
                        deviceBookmark.Deleted = cloudBookmark.Deleted;
                        deviceBookmark.LastChange = DateTime.UtcNow;

                        change = true;
                        bookmarkService.SaveBookmark(deviceBookmark);
                    }
                }

                var cloudMissingBookmarks = deviceBookmarks.Select(o => o.ID).Except(cloudBookmarks.Select(o => o.ID));

                if (cloudMissingBookmarks.Any()) {
                    change = true;
                }

                foreach (var deviceBookmark in deviceBookmarks.Where(o => cloudMissingBookmarks.Contains(o.ID))) {
                    this.SaveBookmark(book.ID, deviceBookmark);
                }

                _bookshelfService.SaveBook(book);

                if (change) {
                    this.SaveBookmarksLastChange(book.ID);
                }

                var bookmarks = await bookmarkService.LoadBookmarksByBookID(book.ID);
                _messageBus.Send(new BookmarksChangedMessage {
                    Bookmarks = bookmarks
                });
            }

        }

        private async void SaveBookmarksLastChange(string bookID) {
            var datetime = DateTime.UtcNow;
            _cloudStorageService.SaveJson(datetime, this.PathGenerator(bookID, BookmarksLastChangeNode));
            var book = await _bookshelfService.LoadBookById(bookID);
            book.BookmarksSyncLastChange = datetime;
            _bookshelfService.SaveBook(book);
        }

        private string[] PathGenerator(string bookID, params string[] nodes) {
            return new string[] { "data", bookID }.Union(nodes).Where(o => !string.IsNullOrEmpty(o)).ToArray();
        }

        private bool CanSync() {
            if (!UserSettings.Synchronization.Enabled) return false;
            if (!CrossConnectivity.Current.IsConnected) return false;
            if (UserSettings.Synchronization.OnlyWifi &&
                !(CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi) ||
                  CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop))
                ) return false;
            if (!_cloudStorageService.IsConnected()) return false;

            return true;
        }
    }
}
