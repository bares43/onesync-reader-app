using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Format;

namespace EbookReader.Service {
    public interface IBookLoader {
        Task<Ebook> GetBook(string filename, byte[] filedata);
        Task<Ebook> OpenBook(string path);
        Task<string> GetChapter(Ebook book, Spine chapter);
        Task<Model.EpubLoader.HtmlResult> PrepareHTML(string html, Ebook book, File chapter);
        Model.Bookshelf.Book CreateBookshelfBook(Ebook book);
    }
}
