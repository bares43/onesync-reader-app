using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EbookReader.DependencyService;

namespace EbookReader.UWP.DependencyService {
    public class UWPAssetsManager : IAssetsManager {
        public async Task<string> GetFileContentAsync(string filename) {
            var assetsPath = string.Format(@"Assets\{0}", filename);
            var storage = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await storage.GetFileAsync(assetsPath).AsTask();
            return await Windows.Storage.FileIO.ReadTextAsync(file).AsTask();
        }

    }
}
