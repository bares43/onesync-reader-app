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

            StyleId = book.Id;

            BindingContext = new {
                Title = book.Title,
                Width = Card.CardWidth,
                Height = Card.CardHeight,
                ImagePosition = new Rectangle(0, 0, Card.CardWidth, Card.CardHeight),
                PanelPosition = new Rectangle(0, 0, Card.CardWidth, 30),
                TextPosition = new Rectangle(0, Card.CardHeight - 70, Card.CardWidth, 70),
                DeletePosition = new Rectangle(Card.CardWidth - 26 - 2, 2, 26, 26)
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
            }
        }

        private void Open() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new OpenBook { Book = _book });
        }

        private void Delete() {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new DeleteBook { Book = _book });
        }

    }
}