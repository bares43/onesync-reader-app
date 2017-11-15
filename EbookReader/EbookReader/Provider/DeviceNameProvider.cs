using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Provider {
    public static class DeviceNameProvider {
        public static string Name {
            get {
                return Device.RuntimePlatform == Device.UWP ? "Počítač" : "Mobil";
            }
        }
    }
}
