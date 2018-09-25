using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Model.Messages;
using EbookReader.Service;
using PCLStorage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Home {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookCard : StackLayout {

        private Model.Bookshelf.Book _book;

        public BookCard(Model.Bookshelf.Book book) {

            _book = book;

            StyleId = book.ID;

            BindingContext = new {
                book.Title,
                Width = Card.CardWidth,
                Height = Card.CardHeight,
                PlaceholderWidth = Card.CardWidth * .75,
                IsFinished = book.FinishedReading.HasValue
            };

            InitializeComponent();

            this.LoadImage();

            DeleteIcon.GestureRecognizers.Add(
                new TapGestureRecognizer {
                    Command = new Command(() => { this.Delete(); })
                }
            );

            GestureRecognizers.Add(
                new TapGestureRecognizer {
                    Command = new Command(() => { this.Open(); }),
                });
        }

        private async void LoadImage() {
            if (!string.IsNullOrEmpty(_book.Cover)) {
                var fileService = IocManager.Container.Resolve<IFileService>();
                var file = await fileService.OpenFile($"{_book.Path}/{_book.Cover}", FileSystem.Current.LocalStorage);
                var stream = await file.OpenAsync(FileAccess.Read);

                Cover.Source = ImageSource.FromStream(() => stream);
                Cover.Aspect = Aspect.Fill;
                Cover.WidthRequest = Card.CardWidth;
                Cover.HeightRequest = Card.CardHeight;
            }
        }

        private void Open() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new OpenBookMessage { Book = _book });
        }

        private void Delete() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new DeleteBookMessage { Book = _book });
        }

    }
}