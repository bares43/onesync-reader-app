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
            var oepbsFolder = await epubFolder.GetFolderAsync("OEBPS");
            var contentFile = await oepbsFolder.GetFileAsync("content.opf");
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

        private async Task<string> LoadEpub(string filename, byte[] filedata) {
            var folderName = filename.Split('.').First();

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
            IFile file = await folder.CreateFileAsync("temp.zip", CreationCollisionOption.OpenIfExists);

            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite)) {
                await stream.WriteAsync(filedata, 0, filedata.Length);
                using (var zf = new ZipFile(stream)) {
                    foreach (ZipEntry zipEntry in zf) {

                        if (zipEntry.IsFile) {
                            var zipEntryStream = zf.GetInputStream(zipEntry);

                            var name = zipEntry.Name;

                            var fileFolder = folder;

                            while (name.Contains("/")) {
                                var parts = name.Split(new char[] { '/' }, 2);
                                var currentFolderName = parts[0];
                                name = parts[1];

                                fileFolder = await folder.CreateFolderAsync(currentFolderName, CreationCollisionOption.OpenIfExists);
                            }

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
