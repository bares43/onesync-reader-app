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
using Plugin.Connectivity;

namespace EbookReader.Service {
    public class SyncService : ISyncService {

        const string ProgressNode = "progress";

        ICloudStorageService _cloudStorageService;

        public SyncService() {
            var service = UserSettings.Synchronization.Enabled ? UserSettings.Synchronization.Service : SynchronizationServicesProvider.Dumb;
            _cloudStorageService = IocManager.Container.ResolveKeyed<ICloudStorageService>(service);
        }

        public async Task<Progress> LoadProgress(string bookID) {

            if (!CanSync()) return null;

            var path = this.PathGenerator(bookID, ProgressNode);

            return await _cloudStorageService.LoadJson<Progress>(path);
        }

        public void SaveProgress(string bookID, Position position) {

            if (!CanSync()) return;

            var progress = new Progress {
                DeviceName = UserSettings.Synchronization.DeviceName,
                Position = position,
            };

            var path = this.PathGenerator(bookID, ProgressNode);

            _cloudStorageService.SaveJson(progress, path);
        }

        private string[] PathGenerator(string bookID, string node) {
            return new string[] { "data", bookID, node };
        }

        private bool CanSync() {
            if (!UserSettings.Synchronization.Enabled) return false;
            if (!CrossConnectivity.Current.IsConnected) return false;
            if (UserSettings.Synchronization.OnlyWifi && 
                !(CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi) ||
                  CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Desktop))
                ) return false;

            return true;
        }
    }
}
