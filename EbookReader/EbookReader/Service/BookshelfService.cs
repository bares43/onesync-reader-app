using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public class BookshelfService : IBookshelfService {

        IEpubLoader _epubLoader;
        IFileService _fileService;

        const string BookshelfFilename = "bookshelf.json";

        public BookshelfService(IEpubLoader epubLoader, IFileService fileService) {
            _epubLoader = epubLoader;
            _fileService = fileService;
        }

        public async void AddBook(FileData file) {

            var epub = await _epubLoader.GetEpub(file.FileName, file.DataArray);

            var book = new Model.Bookshelf.Book {
                Title = epub.Title
            };

            var bookshelf = await this.LoadBookshelf();
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