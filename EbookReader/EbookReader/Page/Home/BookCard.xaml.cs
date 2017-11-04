using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
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

            BindingContext = new {
                Title = book.Title,
                Percents = 58,
            };

            InitializeComponent();

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