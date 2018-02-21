using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using EbookReader.Model.Bookshelf;
using EbookReader.Service;

namespace EbookReader.Repository {
    public class BookRepository : IBookRepository {

        SQLiteAsyncConnection connection;

        public BookRepository(IDatabaseService databaseService) {
            connection = databaseService.Connection;
        }

        public Task<List<Book>> GetAllBooksAsync() {
            return connection.Table<Book>().ToListAsync();
        }

        public Task<Book> GetBookByIDAsync(string id) {
            return connection.Table<Book>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> DeleteBookAsync(Book book) {
            return connection.DeleteAsync(book);
        }

        public Task<int> SaveBookAsync(Book item) {
            return connection.InsertOrReplaceAsync(item);
        }
    }
}
