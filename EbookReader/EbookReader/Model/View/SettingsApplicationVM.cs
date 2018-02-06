using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;

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
    }
}