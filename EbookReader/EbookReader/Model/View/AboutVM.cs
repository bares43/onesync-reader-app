using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class AboutVM : BaseVM {
        public string Version {
            get {
                return "v 1.0.1";
            }
        }

        public ICommand OpenUrlCommand { get; set; }

        public AboutVM() {
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
