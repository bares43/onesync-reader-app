using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace EbookReader.Service {
    public interface IFileService {
        Task<IFile> OpenFile(string name, IFolder folder);
        Task<IFolder> GetFileFolder(string name, IFolder folder);
        string GetLocalFileName(string path);
        Task<string> ReadFileData(string filename);
        Task<string> ReadFileData(string filename, IFolder folder);
        void Save(string path, string content);
        Task<bool> Checkfile(string filename);
        void DeleteFolder(string path);
    }
}
