using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;

namespace EbookReader.Repository {
    public interface IBookRepository {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIDAsync(string id);
        Task<int> DeleteBookAsync(Book book);
        Task<int> SaveBookAsync(Book item);
    }
}
