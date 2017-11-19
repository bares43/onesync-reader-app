using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Page.Settings;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class SettingsSynchronizationVM : BaseVM {

        public SynchronizationServiceVM SynchronizationService { get; set; }

        public FirebaseVM Firebase { get; set; }

        public bool Enabled {
            get => UserSettings.Synchronization.Enabled;
            set {
                if (UserSettings.Synchronization.Enabled == value)
                    return;

                UserSettings.Synchronization.Enabled = value;
                OnPropertyChanged();
            }
        }

        public bool OnlyWifi {
            get => UserSettings.Synchronization.OnlyWifi;
            set {
                if (UserSettings.Synchronization.OnlyWifi == value)
                    return;

                UserSettings.Synchronization.OnlyWifi = value;
                OnPropertyChanged();
            }
        }

        public string DeviceName {
            get => UserSettings.Synchronization.DeviceName;
            set {
                if(UserSettings.Synchronization.DeviceName == value)
                    return;

                UserSettings.Synchronization.DeviceName = value;
                OnPropertyChanged();
            }
        }

        public string DropboxAccessToken {
            get => UserSettings.Synchronization.Dropbox.AccessToken;
            set {
                if(UserSettings.Synchronization.Dropbox.AccessToken == value)
                    return;

                UserSettings.Synchronization.Dropbox.AccessToken = value;
                OnPropertyChanged();
                OnPropertyChanged("IsConnected");
            }
        }
        
        public bool IsConnected {
            get => !string.IsNullOrEmpty(DropboxAccessToken);
        }

        public ICommand ConnectToDropboxCommand { get; set; }
        public ICommand DisconnectDropboxCommand { get; set; }

        public SettingsSynchronizationVM() {
            SynchronizationService = new SynchronizationServiceVM();
            Firebase = new FirebaseVM();
            ConnectToDropboxCommand = new Command(ConnectToDropbox);
            DisconnectDropboxCommand = new Command(DisconnectDropboxAsync);

            IocManager.Container.Resolve<IMessageBus>().Subscribe<DropboxAccessTokenMessage>((msg) => DropboxAccessToken = msg.AccessToken);
        }

        void ConnectToDropbox() {
            IocManager.Container.Resolve<IMessageBus>().Send(new OpenDropboxLogin());
        }

        void DisconnectDropboxAsync() {
            DropboxAccessToken = string.Empty;
        }
    }
}