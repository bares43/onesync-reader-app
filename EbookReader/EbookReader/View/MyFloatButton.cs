using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.View {
    public class MyFloatButton : Xamarin.Forms.View {

        public event EventHandler Clicked;

        public string ButtonBackgroundColor { get; set; }

        public MyFloatButton() {
            ButtonBackgroundColor = AppSettings.Color;
            Margin = new Thickness(0, 0, 20, 20);
        }

        public void TriggerClicked() {
            Clicked?.Invoke(this, new EventArgs());
        }
    }
}
