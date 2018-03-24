using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.View {
    public class SettingsControlVM : BaseVM {

        public PanBrightnessChangeVM PanBrightnessChange { get; set; }
        public VolumeButtonVM VolumeButton { get; set; }

        public SettingsControlVM() {
            PanBrightnessChange = new PanBrightnessChangeVM();
            VolumeButton = new VolumeButtonVM();
        }

        public bool ClickEverywhere {
            get => UserSettings.Control.ClickEverywhere;
            set {
                if (UserSettings.Control.ClickEverywhere == value)
                    return;

                UserSettings.Control.ClickEverywhere = value;
                OnPropertyChanged();
            }
        }

        public bool DoubleSwipe {
            get => UserSettings.Control.DoubleSwipe;
            set {
                if (UserSettings.Control.DoubleSwipe == value)
                    return;

                UserSettings.Control.DoubleSwipe = value;
                OnPropertyChanged();
            }
        }
    }
}
