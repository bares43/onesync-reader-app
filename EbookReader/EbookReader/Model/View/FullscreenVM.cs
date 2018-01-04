using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class FullscreenVM : BaseVM {

        public bool Enabled {
            get => UserSettings.Reader.Fullscreen;
            set {
                if (UserSettings.Reader.Fullscreen == value)
                    return;

                UserSettings.Reader.Fullscreen = value;
                OnPropertyChanged();
            }
        }
    }
}
