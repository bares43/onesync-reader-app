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

        public bool IsConnected() {
            return !string.IsNullOrEmpty(UserSettings.Synchronization.Dropbox.AccessToken);
        }

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
                                body: mem,
                                mute: true);
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

        public async Task<List<T>> LoadJsonList<T>(string[] path) {

            var result = new List<T>();

            try {
                var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;

                if (!string.IsNullOrEmpty(accessToken)) {
                    using (var dbx = new DropboxClient(accessToken)) {

                        var files = await dbx.Files.ListFolderAsync($"/{string.Join("/", path)}");

                        foreach (var file in files.Entries) {

                            var filePath = file.PathLower.Replace(".json", "").Split('/').Where(o => !string.IsNullOrEmpty(o)).ToArray();

                            var bookmark = await this.LoadJson<T>(filePath);

                            result.Add(bookmark);
                        }
                    }
                }
            } catch (DropboxException) { }

            return result;
        }
    }
}