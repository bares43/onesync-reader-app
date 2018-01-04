using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.View {
    public class NightModeVM : BaseVM {

        public bool Enabled {
            get => UserSettings.Reader.NightMode;
            set {
                if (UserSettings.Reader.NightMode == value)
                    return;

                UserSettings.Reader.NightMode = value;
                OnPropertyChanged();
            }
        }
    }
}
