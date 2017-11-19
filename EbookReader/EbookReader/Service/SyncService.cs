using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Sync;
using EbookReader.Provider;
using Newtonsoft.Json;

namespace EbookReader.Service {
    public class SyncService : ISyncService {

        const string ProgressNode = "progress";

        ICloudStorageService _cloudStorageService;

        public SyncService() {
            var service = UserSettings.Synchronization.Enabled ? UserSettings.Synchronization.Service : SynchronizationServicesProvider.Dumb;
            _cloudStorageService = IocManager.Container.ResolveKeyed<ICloudStorageService>(service);
        }

        public async Task<Progress> LoadProgress(string bookID) {
            var path = this.PathGenerator(bookID, ProgressNode);

            return await _cloudStorageService.LoadJson<Progress>(path);
        }

        public void SaveProgress(string bookID, Position position) {

            var progress = new Progress {
                BookID = bookID,
                DeviceName = UserSettings.Synchronization.DeviceName,
                DateTime = DateTime.UtcNow,
                Position = position,
            };

            var path = this.PathGenerator(bookID, ProgressNode);

            _cloudStorageService.SaveJson(progress, path);
        }

        private string[] PathGenerator(string bookID, string node) {
            return new string[] { "data", bookID, node };
        }
    }
}
