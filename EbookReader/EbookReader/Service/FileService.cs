using System;
using System.Collections.Generic;
using System.IO;
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
            if (name.StartsWith("/")) {
                name = name.Substring(1);
            }
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

        public async Task<string> ReadFileData(string filename) {
            return await this.ReadFileData(filename, FileSystem.Current.LocalStorage);
        }

        public async Task<string> ReadFileData(string filename, IFolder folder) {
            var file = await this.OpenFile(filename, folder);
            return await file.ReadAllTextAsync();
        }

        public async void Save(string path, string content) {
            var folder = FileSystem.Current.LocalStorage;
            var file = await folder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
            var bytes = Encoding.UTF8.GetBytes(content);
            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite)) {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public async Task<bool> Checkfile(string filename) {
            var folder = FileSystem.Current.LocalStorage;
            var fileFolder = await this.GetFileFolder(filename, folder);
            var exists = await fileFolder.CheckExistsAsync(this.GetLocalFileName(filename));
            return exists == ExistenceCheckResult.FileExists;
        }

        public async void DeleteFolder(string path) {
            var folder = await FileSystem.Current.LocalStorage.GetFolderAsync(path);
            await folder.DeleteAsync();
        }

    }
}
