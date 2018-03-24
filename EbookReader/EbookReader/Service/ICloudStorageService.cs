using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public interface ICloudStorageService {
        bool IsConnected();
        void SaveJson<T>(T json, string[] path);
        Task<T> LoadJson<T>(string[] path);
        Task<List<T>> LoadJsonList<T>(string[] path);
        void DeleteNode(string[] path);
    }
}
