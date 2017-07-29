using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EbookReader.Model;
using ICSharpCode.SharpZipLib.Zip;
using PCLStorage;

namespace EbookReader.Service {
    public class EpubLoader {
        public async Task<Epub> GetEpub(string filename, byte[] filedata) {
            var folder = await this.LoadEpub(filename, filedata);

            var epubFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(folder);

            var contentFilePath = await GetContentFilePath(epubFolder);

            var contentFile = await this.OpenFile(contentFilePath, epubFolder);
            var contentFileData = await contentFile.ReadAllTextAsync();

            var xml = XDocument.Parse(contentFileData);
            var root = xml.Root;
            var metadata = root.Descendants().Where(o => o.Name.LocalName == "metadata").First();

            var epub = new Epub() {
                Title = metadata.Descendants().Where(o => o.Name.LocalName == "title").First().Value,
                Author = metadata.Descendants().Where(o => o.Name.LocalName == "creator").First().Value,
                Description = metadata.Descendants().Where(o => o.Name.LocalName == "description").First().Value,
            };

            return epub;
        }

        private async Task<string> GetContentFilePath(IFolder epubFolder) {
            var containerFile = await this.OpenFile("META-INF/container.xml", epubFolder);
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

        private async Task<IFile> OpenFile(string name, IFolder folder) {
            folder = await this.GetFileFolder(name, folder);
            return await folder.GetFileAsync(this.GetFileName(name));
        }

        private async Task<IFolder> GetFileFolder(string name, IFolder folder) {
            while (name.Contains("/")) {
                var path = name.Split(new char[] { '/' }, 2);
                var folderName = path[0];
                name = path[1];
                folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        private string GetFileName(string path) {
            return path.Split('/').Last();
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

                            var name = this.GetFileName(zipEntry.Name);

                            var fileFolder = await this.GetFileFolder(zipEntry.Name, folder);
                            
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
