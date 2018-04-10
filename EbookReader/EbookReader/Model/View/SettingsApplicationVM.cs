using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class SettingsApplicationVM : BaseVM {
        public bool AnalyticsAgreement {
            get => UserSettings.AnalyticsAgreement;
            set {
                if (UserSettings.AnalyticsAgreement == value)
                    return;

                UserSettings.AnalyticsAgreement = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenUrlCommand { get; set; }

        public SettingsApplicationVM() {
            OpenUrlCommand = new Command((url) => OpenUrl(url));
        }

        private void OpenUrl(object url) {
            if (url != null) {
                try {
                    var uri = new Uri(url.ToString());
                    Device.OpenUri(uri);
                } catch (Exception) { }
            }
        }
    }
}