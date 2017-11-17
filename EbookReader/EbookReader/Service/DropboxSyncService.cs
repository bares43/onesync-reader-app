using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Sync;
using Newtonsoft.Json;

namespace EbookReader.Service {
    public class DropboxSyncService : ISyncService {

        const string ProgressFilename = "progress.json";

        public async Task<Progress> LoadProgress(string bookID) {
            var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;
            using (var dbx = new DropboxClient(accessToken)) {

                using (var response = await dbx.Files.DownloadAsync($"{GeneratePath(bookID)}/{ProgressFilename}")) {
                    var content = await response.GetContentAsStringAsync();
                    var progress = JsonConvert.DeserializeObject<Progress>(content);
                    return progress;
                }
            }
        }

        public async void SaveProgress(string bookID, Position position) {
            var accessToken = UserSettings.Synchronization.Dropbox.AccessToken;
            using (var dbx = new DropboxClient(accessToken)) {

                var progress = new Progress {
                    BookID = bookID,
                    DeviceName = UserSettings.Synchronization.DeviceName,
                    DateTime = DateTime.UtcNow,
                    Position = position,
                };

                var json = JsonConvert.SerializeObject(progress);

                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(json))) {
                    var updated = await dbx.Files.UploadAsync(
                        $"{GeneratePath(bookID)}/{ProgressFilename}",
                        WriteMode.Overwrite.Instance,
                        body: mem);
                }
            }
        }

        private string GeneratePath(string bookID) {
            return $"/data/{bookID}";
        }

    }
}