using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;

namespace EbookReader.Service {
    public interface IBookmarkService {
        void DeleteBookmark(Bookmark bookmark, string bookID);
        Task<List<Bookmark>> LoadBookmarksByBookID(string bookID);
        void CreateBookmark(string name, string bookID, Position position);
        void SaveBookmark(Bookmark bookmark);
    }
}
