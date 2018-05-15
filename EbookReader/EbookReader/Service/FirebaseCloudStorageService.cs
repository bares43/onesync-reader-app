using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database.Query;
using Microsoft.AppCenter.Crashes;

namespace EbookReader.Service {
    public class FirebaseCloudStorageService : ICloudStorageService {

        public bool IsConnected() {
            return !string.IsNullOrEmpty(UserSettings.Synchronization.Firebase.Email) && !string.IsNullOrEmpty(UserSettings.Synchronization.Firebase.Password);
        }

        public async Task<T> LoadJson<T>(string[] path) {
            try {
                var auth = await this.GetAuth();
                var result = await this.GetFirebase().Child(this.PathGenerator(path, auth)).WithAuth(auth.FirebaseToken).OnceSingleAsync<T>();
                return result;
            } catch { }

            return default(T);
        }

        public async Task<List<T>> LoadJsonList<T>(string[] path) {
            try {
                var auth = await this.GetAuth();
                var result = await this.GetFirebase().Child(this.PathGenerator(path, auth)).WithAuth(auth.FirebaseToken).OnceAsync<T>();
                return result.Select(o => o.Object).ToList();
            } catch { }

            return new List<T>();
        }

        public async void SaveJson<T>(T json, string[] path) {
            try {
                var auth = await this.GetAuth();
                await this.GetFirebase().Child($"{this.PathGenerator(path, auth)}").WithAuth(auth.FirebaseToken).PutAsync(json);
            } catch (Exception e) {
                Crashes.TrackError(e);
            }
        }

        public async void DeleteNode(string[] path) {
            try {
                var auth = await this.GetAuth();
                await this.GetFirebase().Child($"{this.PathGenerator(path, auth)}").WithAuth(auth.FirebaseToken).DeleteAsync();
            } catch { }
        }

        private FirebaseClient GetFirebase() {
            return new FirebaseClient(AppSettings.Synchronization.Firebase.BaseUrl);
        }

        private async Task<FirebaseAuthLink> GetAuth() {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettings.Synchronization.Firebase.ApiKey));
            return await authProvider.SignInWithEmailAndPasswordAsync(UserSettings.Synchronization.Firebase.Email, UserSettings.Synchronization.Firebase.Password);
        }

        private string PathGenerator(string[] path, FirebaseAuthLink auth) {
            return $"users/{auth.User.LocalId}/{string.Join("/", path)}";
        }
    }
}