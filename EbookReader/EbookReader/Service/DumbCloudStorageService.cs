using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public class DumbCloudStorageService : ICloudStorageService {
        public async Task<T> LoadJson<T>(string[] path) {
            return default(T);
        }

        public void SaveJson<T>(T json, string[] path) {
        }
    }
}
