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
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage {

        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;
        IEpubLoader _epubLoader;

        public HomePage() {

            InitializeComponent();

            Init();
        }

        private async void Init() {

            // ioc
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();

            var books = await _bookshelfService.LoadBooks();

            foreach (var book in books) {
                Bookshelf.Children.Add(new BookCard(book));
            }

            Title = "E-book Reader";

            _messageBus.Subscribe<AddBookClicked>(AddBook);
            _messageBus.Subscribe<OpenBook>(OpenBook);
            _messageBus.Subscribe<DeleteBook>(DeleteBook);
        }

        private async void AddBook(AddBookClicked msg) {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            if (pickedFile != null) {

                try {
                    var book = await _bookshelfService.AddBook(pickedFile);
                    Bookshelf.Children.Add(new BookCard(book));
                    this.SendBookToReader(book);
                } catch (Exception) {
                    await DisplayAlert("Chyba", "Soubor se nepodařilo otevřít", "OK");
                }

            }
        }

        private void OpenBook(OpenBook msg) {
            this.SendBookToReader(msg.Book);
        }

        private void DeleteBook(DeleteBook msg) {
            var card = Bookshelf.Children.FirstOrDefault(o => o.StyleId == msg.Book.Id);
            if (card != null) {
                Bookshelf.Children.Remove(card);
            }
            _bookshelfService.RemoveById(msg.Book.Id);
        }

        private async void SendBookToReader(Model.Bookshelf.Book book) {
            var epub = await _epubLoader.OpenEpub(book.Path);

            var page = new ReaderPage();
            page.LoadBook(epub);
            await Navigation.PushAsync(page);
        }

    }
}