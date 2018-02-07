using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Provider;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class PanBrightnessChangeVM : BaseVM {

        public bool Show {
            get {
                return Device.RuntimePlatform == Device.Android;
            }
        }

        public List<string> Items => BrightnessChangeProvider.Items;

        public string Value {
            get => UserSettings.Control.BrightnessChange.ToString();
            set {
                if (Enum.TryParse(value, out BrightnessChange enumValue)) {
                    if (UserSettings.Control.BrightnessChange == enumValue)
                        return;

                    UserSettings.Control.BrightnessChange = enumValue;
                    OnPropertyChanged();
                }
            }
        }
    }
}