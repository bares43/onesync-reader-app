using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Page.Home;
using EbookReader.Service;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage {

        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;

        public HomePage() {
            InitializeComponent();

            Init();
        }

        private async void Init() {

            // ioc
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            var books = await _bookshelfService.LoadBooks();

            foreach(var book in books) {
                Bookshelf.Children.Add(new BookCard(book));
            }
            
            Title = "E-book Reader";

            _messageBus.Subscribe<AddBookClicked>(LoadButton_Clicked);
        }

        private void LoadButton_Clicked(AddBookClicked msg) {
            this.LoadBook();
        }

        public async void LoadBook() {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            if (pickedFile != null) {

                try {

                    var page = App.ReaderPage();
                    page.LoadBook(pickedFile);

                    await Navigation.PushAsync(page);

                } catch (Exception) {
                    await DisplayAlert("Chyba", "Soubor se nepodařilo otevřít", "OK");
                }

            }
        }

    }
}