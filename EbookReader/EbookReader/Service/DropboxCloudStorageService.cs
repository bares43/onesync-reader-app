using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json;

namespace EbookReader.Service {
    public class DropboxCloudStorageService : ICloudStorageService {

        const string ProgressFilename = "progress.json";

        public async Task<T> LoadJson<T>(string[] path) {

            try {
                var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;

                if (!string.IsNullOrEmpty(accessToken)) {
                    using (var dbx = new DropboxClient(accessToken)) {
                        using (var response = await dbx.Files.DownloadAsync($"/{string.Join("/", path)}.json")) {
                            var json = await response.GetContentAsStringAsync();
                            return JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                }
            } catch (DropboxException) { }


            return default(T);
        }

        public async void SaveJson<T>(T json, string[] path) {

            try {
                var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;

                if (!string.IsNullOrEmpty(accessToken)) {

                    using (var dbx = new DropboxClient(accessToken)) {
                        using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json)))) {
                            await dbx.Files.UploadAsync(
                                $"/{string.Join("/", path)}.json",
                                WriteMode.Overwrite.Instance,
                                body: mem);
                        }
                    }
                }
            } catch (DropboxException) { }
        }

        public async void DeleteNode(string[] path) {

            try {
                var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;

                if (!string.IsNullOrEmpty(accessToken)) {
                    using (var dbx = new DropboxClient(accessToken)) {
                        await dbx.Files.DeleteV2Async($"/{string.Join("/", path)}");
                    }
                }

            } catch (DropboxException) { }
        }
    }
}