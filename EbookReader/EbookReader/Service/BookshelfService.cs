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

        public async void AddBook(FileData file) {

            var epub = await _epubLoader.GetEpub(file.FileName, file.DataArray);

            var b64 = Convert.ToBase64String(file.DataArray);

            var book = new Model.Bookshelf.Book {
                Id = _cryptoService.GetMd5(file.DataArray),
                Title = epub.Title,
                Created = DateTime.UtcNow,
                Path = epub.Folder,
            };

            var bookshelf = await this.LoadBookshelf();

            if (!bookshelf.Books.Any(o => o.Id == book.Id)) {
                bookshelf.Books.Add(book);
                this.Save(bookshelf);
            }
        }
        
        public async Task<List<Model.Bookshelf.Book>> LoadBooks() {
            var bookshelf = await this.LoadBookshelf();
            return bookshelf.Books;
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