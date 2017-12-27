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
        IEpubLoader _epubLoader;
        ISyncService _syncService;

        public HomePage() {

            InitializeComponent();

            Init();

            var settingsItem = new ToolbarItem {
                Text = "Nastavení",
                Icon = "settings.png"
            };
            settingsItem.Clicked += SettingsItem_Clicked;
            ToolbarItems.Add(settingsItem);

            var aboutItem = new ToolbarItem {
                Text = "O aplikaci",
                Icon = "info.png"
            };
            aboutItem.Clicked += AboutItem_Clicked;

            ToolbarItems.Add(aboutItem);
        }

        private async void AboutItem_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.AboutPage());
        }

        private async void SettingsItem_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(App.SettingsPage());
        }

        private async void Init() {

            // ioc
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
            _epubLoader = IocManager.Container.Resolve<IEpubLoader>();
            _syncService = IocManager.Container.Resolve<ISyncService>();

            var books = await _bookshelfService.LoadBooks();

            foreach (var book in books) {
                Bookshelf.Children.Add(new BookCard(book));
            }

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

        private async void DeleteBook(DeleteBook msg) {
            var deleteButton = "Smazat";
            var deleteSyncButton = "Smazat včetně všech synchronizací";
            var confirm = await DisplayActionSheet("Smazat knihu z knihovny?", deleteButton, "Ne", deleteSyncButton);
            if (confirm == deleteButton || confirm == deleteSyncButton) {
                var card = Bookshelf.Children.FirstOrDefault(o => o.StyleId == msg.Book.Id);
                if (card != null) {
                    Bookshelf.Children.Remove(card);
                }
                _bookshelfService.RemoveById(msg.Book.Id);

                if(confirm == deleteSyncButton) {
                    _syncService.DeleteBook(msg.Book.Id);
                }
            }
        }

        private async void SendBookToReader(Model.Bookshelf.Book book) {
            var page = new ReaderPage();
            page.LoadBook(book);
            await Navigation.PushAsync(page);
        }

        private void OpenButton_Clicked(object sender, EventArgs e) {
            _messageBus.Send(new AddBookClicked());
        }
    }
}