using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using PCLStorage;
using Rg.Plugins.Popup.Extensions;
using Xam.Plugin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Home {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookCard : StackLayout {

        private Model.Bookshelf.Book _book;

        public BookCard(Model.Bookshelf.Book book) {

            _book = book;

            StyleId = book.Id;

            BindingContext = new {
                Title = book.Title,
                Percents = 58,
            };

            InitializeComponent();

            this.LoadImage();

            Panel.GestureRecognizers.Add(
                new TapGestureRecognizer {
                    NumberOfTapsRequired = 1,               
                    Command = new Command(() => { this.Delete(); })
                }
            );

            GestureRecognizers.Add(
                new TapGestureRecognizer {
                    NumberOfTapsRequired = 1,
                    Command = new Command(() => { this.Open(); }),
                });
        }

        private async void LoadImage() {
            var fileService = IocManager.Container.Resolve<IFileService>();
            var file = await fileService.OpenFile("zaklinac-i-posledni-prani/OEBPS/Images/cover.jpg", FileSystem.Current.LocalStorage);
            var stream = await file.OpenAsync(FileAccess.Read);
            
            Cover.Source = ImageSource.FromStream(() => stream);

        }

        private void Open() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new OpenBook { Book = _book });
        }

        private async void Delete() {
            var page = new Popup(_book);
            await Navigation.PushPopupAsync(page);
        }

    }
}