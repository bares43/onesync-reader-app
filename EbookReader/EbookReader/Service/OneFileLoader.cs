using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.EpubLoader;
using EbookReader.Model.Format;
using HtmlAgilityPack;
using PCLStorage;

namespace EbookReader.Service {
    public abstract class OneFileLoader : IBookLoader {

        private IFileService _fileService;

        protected string[] Extensions;

        protected string ContentPath => $"content.{Extensions[0]}";

        protected string TitlePath => "title";

        protected EbookFormat EbookFormat;

        public OneFileLoader(IFileService fileService) {
            _fileService = fileService;
        }

        public virtual Book CreateBookshelfBook(Ebook book) {
            return new Book {
                Title = book.Title,
                Format = EbookFormat,
                Path = book.Folder,
            };
        }

        public virtual async Task<Ebook> GetBook(string filename, byte[] filedata, string bookID) {
            var folder = await this.LoadEpub(filename, filedata, bookID);

            return await OpenBook(folder);
        }

        public virtual async Task<string> GetChapter(Ebook book, Spine chapter) {
            var folder = await FileSystem.Current.LocalStorage.GetFolderAsync(book.Folder);
            return await _fileService.ReadFileData(ContentPath, folder);
        }

        public virtual async Task<Ebook> OpenBook(string path) {
            var folder = await FileSystem.Current.LocalStorage.GetFolderAsync(path);

            var titleFile = await folder.GetFileAsync(TitlePath);
            var title = await titleFile.ReadAllTextAsync();

            var epub = new Ebook() {
                Title = title,
                Spines = new List<Spine>() { new Spine { Idref = "content" } },
                Files = new List<File>() { new File { Id = "content", Href = ContentPath } },
                Folder = path,
                Navigation = new List<Model.Navigation.Item>(),
            };

            return epub;
        }

        public virtual async Task<HtmlResult> PrepareHTML(string html, Ebook book, File chapter) {

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            this.StripHtmlTags(doc);

            html = doc.DocumentNode.Descendants("body").First().InnerHtml;

            return await Task.Run(() => {
                var result = new HtmlResult {
                    Html = html,
                    Images = new List<Image>(),
                };

                return result;
            });
        }

        protected virtual async Task<string> LoadEpub(string filename, byte[] filedata, string bookID) {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(bookID, CreationCollisionOption.ReplaceExisting);
            var contentFile = await folder.CreateFileAsync(ContentPath, CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await contentFile.OpenAsync(FileAccess.ReadAndWrite)) {
                await stream.WriteAsync(filedata, 0, filedata.Length);
            }

            var titleFile = await folder.CreateFileAsync(TitlePath, CreationCollisionOption.ReplaceExisting);
            await titleFile.WriteAllTextAsync(filename.Split('.').First());

            return folder.Name;
        }

        protected virtual void StripHtmlTags(HtmlDocument doc) {
            var tagsToRemove = new string[] { "script", "style", "iframe" };
            var nodesToRemove = doc.DocumentNode
                .Descendants()
                .Where(o => tagsToRemove.Contains(o.Name))
                .ToList();

            foreach (var node in nodesToRemove) {
                node.Remove();
            }
        }
    }
}
