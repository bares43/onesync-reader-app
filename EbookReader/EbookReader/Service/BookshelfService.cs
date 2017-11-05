using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.DependencyService;
using Newtonsoft.Json;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public class BookshelfService : IBookshelfService {

        IEpubLoader _epubLoader;
        IFileService _fileService;
        ICryptoService _cryptoService;

        const string BookshelfFilename = "bookshelf.json";

        public BookshelfService(IEpubLoader epubLoader, IFileService fileService, ICryptoService cryptoService) {
            _epubLoader = epubLoader;
            _fileService = fileService;
            _cryptoService = cryptoService;
        }

        public async Task<Model.Bookshelf.Book> AddBook(FileData file) {

            var epub = await _epubLoader.GetEpub(file.FileName, file.DataArray);

            var id = _cryptoService.GetMd5(file.DataArray);

            var bookshelf = await this.LoadBookshelf();

            var book = bookshelf.Books.FirstOrDefault(o => o.Id == id);
            if (book == null) {
                book = new Model.Bookshelf.Book {
                    Id = _cryptoService.GetMd5(file.DataArray),
                    Title = epub.Title,
                    Created = DateTime.UtcNow,
                    Path = epub.Folder,
                };
                bookshelf.Books.Add(book);
                this.Save(bookshelf);
            }

            return book;
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