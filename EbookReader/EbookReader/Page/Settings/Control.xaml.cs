using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Settings {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Control : ContentPage {
        public Control() {
            InitializeComponent();

            BindingContext = new SettingsControlVM();
        }
    }
}