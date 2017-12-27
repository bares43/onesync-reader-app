using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Reader {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPanel : StackLayout {
        public InfoPanel() {
            InitializeComponent();

            BindingContext = new Model.View.InfoPanelVM();
        }
    }
}