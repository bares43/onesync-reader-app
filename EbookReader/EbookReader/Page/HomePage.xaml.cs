using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Page.Home;
using EbookReader.Service;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage {

        IBookshelfService _bookshelfService;
        IMessageBus _messageBus;
        ISyncService _syncService;

        public HomePage() {

            InitializeComponent();

            Init();

            if (!App.HasMasterDetailPage) {

                var settingsItem = new ToolbarItem {
                    Text = "Settings",
                    Icon = "settings.png"
                };
                settingsItem.Clicked += SettingsItem_Clicked;
                ToolbarItems.Add(settingsItem);

                var aboutItem = new ToolbarItem {
                    Text = "About",
                    Icon = "info.png",
                };
                aboutItem.Clicked += AboutItem_Clicked;

                ToolbarItems.Add(aboutItem);

            }
        }

        protected override void OnAppearing() {
            base.OnAppearing();

            // because of floating action button on android
            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), () => {
                _messageBus.Send(new FullscreenRequestMessage(false));
                return false;
            });

            this.ShowAnalyticsAgreement();

            UserSettings.FirstRun = false;
        }

        private async void AboutItem_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new AboutPage());
        }

        private async void SettingsItem_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void ShowAnalyticsAgreement() {
            if (UserSettings.FirstRun) {
                var result = await DisplayAlert("Agreement with collection of anonymous data", "I agree with collecting of anonymous information about using of the app. This is important for application improvements.", "I agree", "No");
                UserSettings.AnalyticsAgreement = result;
            }
        }

        private async void Init() {

            // ioc
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
            _syncService = IocManager.Container.Resolve<ISyncService>();

            var books = await _bookshelfService.LoadBooks();

            foreach (var book in books) {
                Bookshelf.Children.Add(new BookCard(book));
            }

            _messageBus.Subscribe<AddBookClickedMessage>(AddBook);
            _messageBus.Subscribe<OpenBookMessage>(OpenBook);
            _messageBus.Subscribe<DeleteBookMessage>(DeleteBook);
        }

        private async void AddBook(AddBookClickedMessage msg) {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            if (pickedFile != null) {

                try {
                    var book = await _bookshelfService.AddBook(pickedFile);
                    if (book.Item2) {
                        Bookshelf.Children.Add(new BookCard(book.Item1));
                    }
                    this.SendBookToReader(book.Item1);
                } catch (Exception e) {
                    var ext = string.Empty;
                    if (!string.IsNullOrEmpty(pickedFile.FileName)) {
                        ext = pickedFile.FileName.Split('.').LastOrDefault();
                    }
                    Analytics.TrackEvent("Failed to open book", new Dictionary<string, string> {
                        { "Extension", ext }
                    });
                    Crashes.TrackError(e, new Dictionary<string, string> {
                        { "Filename", pickedFile.FileName }
                    });
                    await DisplayAlert("Error", "File failed to open", "OK");
                }

            }
        }

        private void OpenBook(OpenBookMessage msg) {
            this.SendBookToReader(msg.Book);
        }

        private async void DeleteBook(DeleteBookMessage msg) {
            var deleteButton = "Delete";
            var deleteSyncButton = "Delete including all synchronizations";
            var confirm = await DisplayActionSheet("Delete book?", deleteButton, "No", deleteSyncButton);
            if (confirm == deleteButton || confirm == deleteSyncButton) {
                var card = Bookshelf.Children.FirstOrDefault(o => o.StyleId == msg.Book.ID);
                if (card != null) {
                    Bookshelf.Children.Remove(card);
                }
                _bookshelfService.RemoveById(msg.Book.ID);

                if (confirm == deleteSyncButton) {
                    _syncService.DeleteBook(msg.Book.ID);
                }
            }
        }

        private async void SendBookToReader(Model.Bookshelf.Book book) {
            var page = new ReaderPage();
            page.LoadBook(book);
            await Navigation.PushAsync(page);
        }

        private void MyFloatButton_Clicked(object sender, EventArgs e) {
            _messageBus.Send(new AddBookClickedMessage());
        }
    }
}