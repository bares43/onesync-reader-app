using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace EbookReader.Service {
    public class FileService : IFileService {
        public async Task<IFile> OpenFile(string name, IFolder folder) {
            folder = await this.GetFileFolder(name, folder);
            return await folder.GetFileAsync(this.GetLocalFileName(name));
        }

        public async Task<IFolder> GetFileFolder(string name, IFolder folder) {
            while (name.Contains("/")) {
                var path = name.Split(new char[] { '/' }, 2);
                var folderName = path[0];
                name = path[1];
                folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        public string GetLocalFileName(string path) {
            return path.Split('/').Last();
        }

        public async Task<string> ReadFileData(string filename, IFolder folder) {
            var file = await this.OpenFile(filename, folder);
            return await file.ReadAllTextAsync();
        }
        
    }
}
