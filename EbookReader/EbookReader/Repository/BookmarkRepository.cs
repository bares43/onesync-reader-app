using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using EbookReader.Service;
using SQLite;

namespace EbookReader.Repository {
    public class BookmarkRepository : IBookmarkRepository {

        SQLiteAsyncConnection connection;

        public BookmarkRepository(IDatabaseService databaseService) {
            connection = databaseService.Connection;
        }

        public Task<List<Bookmark>> GetBookmarksByBookIDAsync(string bookID) {
            return connection.Table<Bookmark>().Where(o => o.BookID == bookID).ToListAsync();
        }
        
        public Task<int> DeleteBookmarkAsync(Bookmark bookmark) {
            return connection.DeleteAsync(bookmark);
        }

        public Task<int> SaveBookmarkAsync(Bookmark bookmark) {
            return connection.InsertOrReplaceAsync(bookmark);
        }
    }
}