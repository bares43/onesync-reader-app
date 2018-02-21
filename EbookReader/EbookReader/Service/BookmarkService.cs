using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using EbookReader.Provider;
using EbookReader.Repository;

namespace EbookReader.Service {
    public class BookmarkService : IBookmarkService {

        IBookmarkRepository _bookmarkRepository;
        IBookRepository _bookRepository;
        ISyncService _syncService;

        public BookmarkService(IBookmarkRepository bookmarkRepository, IBookRepository bookRepository, ISyncService syncService) {
            _bookmarkRepository = bookmarkRepository;
            _bookRepository = bookRepository;
            _syncService = syncService;
        }

        public void CreateBookmark(string name, string bookID, Position position) {
            var bookmark = new Bookmark {
                ID = BookmarkIdProvider.ID,
                Name = name,
                Position = new Position(position),
                BookID = bookID,
            };

            this.SaveBookmark(bookmark);
            _syncService.SaveBookmark(bookID, bookmark);
        }

        public async void DeleteBookmark(Bookmark bookmark, string bookID) {
            bookmark.Deleted = true;
            bookmark.Name = string.Empty;
            bookmark.Position = new Position();

            await _bookmarkRepository.SaveBookmarkAsync(bookmark);
            _syncService.SaveBookmark(bookID, bookmark);
        }

        public async Task<List<Bookmark>> LoadBookmarksByBookID(string bookID) {
            return await _bookmarkRepository.GetBookmarksByBookIDAsync(bookID);
        }

        public async void SaveBookmark(Bookmark bookmark) {
            bookmark.LastChange = DateTime.UtcNow;
            await _bookmarkRepository.SaveBookmarkAsync(bookmark);
        }
    }
}
