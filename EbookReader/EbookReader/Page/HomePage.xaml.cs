using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage {
        public HomePage() {
            InitializeComponent();
        }
        
        private void LoadButton_Clicked(object sender, EventArgs e) {
            this.LoadBook();
        }

        public async void LoadBook() {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            if (pickedFile != null) {

                try {

                    var page = App.ReaderPage();
                    await page.LoadBook(pickedFile);

                    await Navigation.PushAsync(page);

                } catch (Exception) {
                    await DisplayAlert("Chyba", "Soubor se nepodařilo otevřít", "OK");
                }

            }
        }

    }
}