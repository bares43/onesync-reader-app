using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Helpers;
using EbookReader.Model.Bookshelf;
using EbookReader.Repository;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public class BookshelfService : IBookshelfService {

        IFileService _fileService;
        ICryptoService _cryptoService;
        IBookRepository _bookRepository;
        IBookmarkRepository _bookmarkRepository;

        public BookshelfService(IFileService fileService, ICryptoService cryptoService, IBookRepository bookRepository, IBookmarkRepository bookmarkRepository) {
            _fileService = fileService;
            _cryptoService = cryptoService;
            _bookRepository = bookRepository;
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<Tuple<Book, bool>> AddBook(FileData file) {

            var newBook = false;
            var bookLoader = EbookFormatHelper.GetBookLoader(file.FileName);

            var id = _cryptoService.GetMd5(file.DataArray);

            var bookshelfBook = await _bookRepository.GetBookByIDAsync(id);

            if (bookshelfBook == null) {
                var ebook = await bookLoader.GetBook(file.FileName, file.DataArray, id);
                bookshelfBook = bookLoader.CreateBookshelfBook(ebook);
                bookshelfBook.ID = id;
                await _bookRepository.SaveBookAsync(bookshelfBook);
                newBook = true;
            }

            return new Tuple<Book, bool>(bookshelfBook, newBook);
        }

        public async Task<List<Book>> LoadBooks() {
            return await _bookRepository.GetAllBooksAsync();
        }
        
        public async void RemoveById(string id) {
            var book = await _bookRepository.GetBookByIDAsync(id);
            if (book != null) {
                _fileService.DeleteFolder(book.Path);
                var bookmarks = await _bookmarkRepository.GetBookmarksByBookIDAsync(id);
                foreach(var bookmark in bookmarks) {
                    await _bookmarkRepository.DeleteBookmarkAsync(bookmark);
                }
                await _bookRepository.DeleteBookAsync(book);
            }
        }

        public async void SaveBook(Book book) {
            await _bookRepository.SaveBookAsync(book);
        }

        public async Task<Book> LoadBookById(string id) {
            return await _bookRepository.GetBookByIDAsync(id);
        }
    }
}