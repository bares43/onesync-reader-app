using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;

namespace EbookReader.Repository {
    public interface IBookmarkRepository {
        Task<List<Bookmark>> GetBookmarksByBookIDAsync(string bookID);
        Task<int> DeleteBookmarkAsync(Bookmark book);
        Task<int> SaveBookmarkAsync(Bookmark item);
    }
}
