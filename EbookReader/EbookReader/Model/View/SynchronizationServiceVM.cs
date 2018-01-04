using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Provider;

namespace EbookReader.Model.View {
    public class SynchronizationServiceVM : BaseVM {
        
        public List<string> Items => SynchronizationServicesProvider.Items;

        public string Value {
            get => UserSettings.Synchronization.Service;
            set {
                if (UserSettings.Synchronization.Service == value)
                    return;

                UserSettings.Synchronization.Service = value;
                OnPropertyChanged();
            }
        }
    }
}
