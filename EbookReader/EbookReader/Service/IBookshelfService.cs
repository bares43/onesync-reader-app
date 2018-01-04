using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public interface IBookshelfService {
        Task<Model.Bookshelf.Book> AddBook(FileData file);
        Task<List<Model.Bookshelf.Book>> LoadBooks();
        Task<Model.Bookshelf.Book> LoadBookById(string id);
        void RemoveById(string id);
        void SaveBook(Model.Bookshelf.Book book);
    }
}
