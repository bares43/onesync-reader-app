using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using EbookReader.DependencyService;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;
using Microsoft.AppCenter.Analytics;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class FirebaseVM : BaseVM {
        public string Email {
            get => UserSettings.Synchronization.Firebase.Email;
            set {
                if (UserSettings.Synchronization.Firebase.Email == value)
                    return;

                Password = string.Empty;
                UserSettings.Synchronization.Firebase.Email = value;
                OnPropertyChanged();
            }
        }

        public string Password {
            get => UserSettings.Synchronization.Firebase.Password;
            set {
                if (UserSettings.Synchronization.Firebase.Password == value)
                    return;

                UserSettings.Synchronization.Firebase.Password = value;
                OnPropertyChanged();

            }
        }

        bool _isConnected = false;
        public bool IsConnected {
            get => _isConnected;
            set {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        bool _loginFailed = false;
        public bool LoginFailed {
            get => _loginFailed;
            set {
                _loginFailed = value;
                OnPropertyChanged();
            }
        }

        bool _resetMailSent = false;
        public bool ResetMailSent {
            get => _resetMailSent;
            set {
                _resetMailSent = value;
                OnPropertyChanged();
            }
        }

        bool _mailNotFound = false;
        public bool MailNotFound {
            get => _mailNotFound;
            set {
                _mailNotFound = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public FirebaseVM() {
            ConnectCommand = new Command(Connect);
            DisconnectCommand = new Command(Disconnect);
            ResetCommand = new Command(Reset);

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password)) {
                Connect();
            }
        }

        private void Disconnect() {
            Email = string.Empty;
            IsConnected = false;
        }

        private async void Connect() {
            if (!CrossConnectivity.Current.IsConnected) {
                IocManager.Container.Resolve<IToastService>().Show("There is no Internet connection.");

                return;
            }

            var client = new FirebaseClient(AppSettings.Synchronization.Firebase.BaseUrl);
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettings.Synchronization.Firebase.ApiKey));

            var connected = await this.TrySignIn();
            if (!connected) {
                connected = await this.TryCreate();
            }

            if (connected) {
                Analytics.TrackEvent("Firebase login successful");
            } else {
                Analytics.TrackEvent("Firebase login failed");
            }

            IsConnected = connected;
            LoginFailed = !connected;
            MailNotFound = false;
            ResetMailSent = false;
        }

        private async Task<bool> TrySignIn() {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettings.Synchronization.Firebase.ApiKey));

            var success = false;

            try {
                await authProvider.SignInWithEmailAndPasswordAsync(Email, Password);
                success = true; ;
            } catch (Exception) { }

            return success;
        }

        private async Task<bool> TryCreate() {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettings.Synchronization.Firebase.ApiKey));

            var success = false;

            try {
                await authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);
                success = true; ;
            } catch (Exception) { }

            return success;
        }

        private async void Reset() {
            ResetMailSent = false;
            MailNotFound = false;
            try {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettings.Synchronization.Firebase.ApiKey));
                await authProvider.SendPasswordResetEmailAsync(Email);
                ResetMailSent = true;
            } catch (Exception e) {
                MailNotFound = true;
            }
        }

    }
}
