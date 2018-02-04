using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Exceptions;
using EbookReader.Helpers;
using EbookReader.Model.Format;
using Newtonsoft.Json;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public class BookshelfService : IBookshelfService {

        IFileService _fileService;
        ICryptoService _cryptoService;

        const string BookshelfFilename = "bookshelf.json";

        public BookshelfService(IFileService fileService, ICryptoService cryptoService) {
            _fileService = fileService;
            _cryptoService = cryptoService;
        }

        public async Task<Model.Bookshelf.Book> AddBook(FileData file) {

            var bookLoader = EbookFormatHelper.GetBookLoader(file.FileName);

            var ebook = await bookLoader.GetBook(file.FileName, file.DataArray);

            var bookshelf = await this.LoadBookshelf();

            var id = _cryptoService.GetMd5(file.DataArray);

            var bookshelfBook = bookshelf.Books.FirstOrDefault(o => o.Id == id);
            if (bookshelfBook == null) {
                bookshelfBook = bookLoader.CreateBookshelfBook(ebook);
                bookshelfBook.Id = id;
                bookshelf.Books.Add(bookshelfBook);
                this.Save(bookshelf);
            }

            return bookshelfBook;
        }

        public async Task<List<Model.Bookshelf.Book>> LoadBooks() {
            var bookshelf = await this.LoadBookshelf();
            return bookshelf.Books;
        }

        public async Task<Model.Bookshelf.Book> LoadBookById(string id) {
            var bookshelf = await this.LoadBookshelf();
            return bookshelf.Books.FirstOrDefault(o => o.Id == id);
        }

        public async void RemoveById(string id) {
            var bookshelf = await this.LoadBookshelf();
            var book = bookshelf.Books.FirstOrDefault(o => o.Id == id);
            if (book != null) {
                bookshelf.Books.Remove(book);
                this.Save(bookshelf);
                _fileService.DeleteFolder(book.Path);
            }
        }

        public async void SaveBook(Model.Bookshelf.Book book) {
            var bookshelf = await this.LoadBookshelf();
            var currentBook = bookshelf.Books.FirstOrDefault(o => o.Id == book.Id);
            if(currentBook != null) {
                bookshelf.Books.Remove(currentBook);
            }
            bookshelf.Books.Add(book);
            this.Save(bookshelf);
        }

        private async Task<Model.Bookshelf.Bookshelf> LoadBookshelf() {
            var bookshelf = new Model.Bookshelf.Bookshelf();

            if (await _fileService.Checkfile(BookshelfFilename)) {
                var fileData = await _fileService.ReadFileData(BookshelfFilename);

                if (!string.IsNullOrEmpty(fileData)) {
                    bookshelf = JsonConvert.DeserializeObject<Model.Bookshelf.Bookshelf>(fileData);
                }
            }

            return bookshelf;
        }

        private void Save(Model.Bookshelf.Bookshelf bookshelf) {
            var json = JsonConvert.SerializeObject(bookshelf);
            _fileService.Save(BookshelfFilename, json);
        }
    }
}