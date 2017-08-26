using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autofac;
using EbookReader.Helpers;
using EbookReader.Model;
using EbookReader.Service.Epub;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using PCLStorage;

namespace EbookReader.Service {
    public class EpubLoader : IEpubLoader {

        private IFileService _fileService;

        public EpubLoader(IFileService fileService) {
            _fileService = fileService;
        }

        public async Task<Model.Epub> GetEpub(string filename, byte[] filedata) {
            var folder = await this.LoadEpub(filename, filedata);

            var epubFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(folder);

            var contentFilePath = await this.GetContentFilePath(epubFolder);

            var contentFileData = await _fileService.ReadFileData(contentFilePath, epubFolder);

            var xml = XDocument.Parse(contentFileData);

            var package = xml.Root;

            var epubVersion = this.GetEpubVersion(package);

            var epubParser = IocManager.Container.ResolveKeyed<EpubParser>(epubVersion, new NamedParameter("package", package));

            var epub = new Model.Epub() {
                Version = epubVersion,
                Title = epubParser.GetTitle(),
                Author = epubParser.GetAuthor(),
                Description = epubParser.GetDescription(),
                Language = epubParser.GetLanguage(),
                Spines = epubParser.GetSpines(),
                Files = epubParser.GetFiles(),
                Folder = folder,
            };

            return epub;
        }

        public async Task<string> GetChapter(Model.Epub epub, EpubSpine chapter) {
            var filename = epub.Files.Where(o => o.Id == chapter.Idref).First();
            var folder = await FileSystem.Current.LocalStorage.GetFolderAsync(epub.Folder);
            return await _fileService.ReadFileData(string.Format("OEBPS/{0}", filename.Href), folder);
        }

        public async Task<Model.EpubLoader.HtmlResult> PrepareHTML(string html, string epubFolderName) {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            this.StripHtmlTags(doc);

            var images = await this.PrepareHtmlImages(doc, epubFolderName);

            var result = new Model.EpubLoader.HtmlResult {
                Html = doc.DocumentNode.Descendants("body").First().InnerHtml,
                Images = images,
            };

            return result;
        }

        private void StripHtmlTags(HtmlDocument doc) {
            var tagsToRemove = new string[] { "script", "style", "iframe" };
            var nodesToRemove = doc.DocumentNode
                .Descendants()
                .Where(o => tagsToRemove.Contains(o.Name))
                .ToList();

            foreach (var node in nodesToRemove) {
                node.Remove();
            }
        }

        private async Task<List<Model.EpubLoader.Image>> PrepareHtmlImages(HtmlDocument doc, string epubFolderName) {
            var imagesModel = this.GetImages(doc);

            return await this.ReplaceImagesWithBase64(imagesModel, epubFolderName);
        }

        private async Task<List<Model.EpubLoader.Image>> ReplaceImagesWithBase64(List<Model.EpubLoader.Image> imagesModel, string epubFolderName) {
            var epubFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(epubFolderName);

            foreach (var imageModel in imagesModel) {
                var extension = imageModel.FileName.Split('.').Last();

                var fileName = string.Format("OEBPS/{0}", imageModel.FileName.Replace("../", "")).Replace("//", "/").Replace("%20", " ");

                var file = await _fileService.OpenFile(fileName, epubFolder);

                using (var stream = await file.OpenAsync(FileAccess.Read)) {
                    var base64 = Base64Helper.GetFileBase64(stream);

                    imageModel.Data = string.Format("data:image/{0};base64,{1}", extension, base64);
                }

            }

            return imagesModel;
        }

        private List<Model.EpubLoader.Image> GetImages(HtmlDocument doc) {
            var images = doc.DocumentNode.Descendants("img").ToList();
            var imagesModel = new List<Model.EpubLoader.Image>();

            var cnt = 1;
            foreach (var image in images) {
                var srcAttribute = image.Attributes.FirstOrDefault(o => o.Name == "src");

                if (srcAttribute != null) {
                    int id;

                    var existingImageModel = imagesModel.FirstOrDefault(o => o.FileName == srcAttribute.Value);

                    if (existingImageModel != null) {
                        id = existingImageModel.ID;
                    } else {
                        id = cnt;
                        imagesModel.Add(new Model.EpubLoader.Image {
                            ID = id,
                            FileName = srcAttribute.Value,
                        });

                        cnt++;
                    }

                    image.Attributes.Add(doc.CreateAttribute("data-js-ebook-image-id", id.ToString()));
                }
            }

            return imagesModel;
        }
        
        private EpubVersion GetEpubVersion(XElement package) {
            var version = package.Attributes().First(o => o.Name.LocalName == "version").Value;
            return EpubVersionHelper.ParseVersion(version);
        }

        private async Task<string> GetContentFilePath(IFolder epubFolder) {
            var containerFile = await _fileService.OpenFile("META-INF/container.xml", epubFolder);
            var containerFileContent = await containerFile.ReadAllTextAsync();
            var xmlContainer = XDocument.Parse(containerFileContent);
            var contentFilePath = xmlContainer.Root
                .Descendants()
                .First(o => o.Name.LocalName == "rootfiles")
                .Descendants()
                .First(o => o.Name.LocalName == "rootfile")
                .Attributes()
                .First(o => o.Name.LocalName == "full-path")
                .Value;
            return contentFilePath;
        }

        private async Task<string> LoadEpub(string filename, byte[] filedata) {
            var folderName = filename.Split('.').First();

            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
            var file = await folder.CreateFileAsync("temp.zip", CreationCollisionOption.OpenIfExists);

            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite)) {
                await stream.WriteAsync(filedata, 0, filedata.Length);
                using (var zf = new ZipFile(stream)) {
                    foreach (ZipEntry zipEntry in zf) {

                        if (zipEntry.IsFile) {
                            var zipEntryStream = zf.GetInputStream(zipEntry);

                            var name = _fileService.GetLocalFileName(zipEntry.Name);

                            var fileFolder = await _fileService.GetFileFolder(zipEntry.Name, folder);

                            IFile zipEntryFile = await fileFolder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
                            var str = zf.GetInputStream(zipEntry);
                            using (Stream outPutFileStream = await zipEntryFile.OpenAsync(FileAccess.ReadAndWrite)) {
                                await str.CopyToAsync(outPutFileStream);
                            }
                        }
                    }
                }
            }

            return folder.Name;
        }
    }
}
