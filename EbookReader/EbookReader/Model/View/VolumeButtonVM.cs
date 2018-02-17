using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class VolumeButtonVM : BaseVM {
        public bool Show {
            get {
                return Device.RuntimePlatform == Device.Android;
            }
        }

        public bool Enabled {
            get => UserSettings.Control.VolumeButtons;
            set {
                if (UserSettings.Control.VolumeButtons == value)
                    return;

                UserSettings.Control.VolumeButtons = value;
                OnPropertyChanged();
            }
        }
    }
}
