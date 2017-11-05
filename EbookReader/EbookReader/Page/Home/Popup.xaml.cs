using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Home {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup : PopupPage {

        IMessageBus _messageBus;
        Model.Bookshelf.Book _book;

        public Popup(Model.Bookshelf.Book book) {

            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            _book = book;

            BindingContext = new {
                Title = book.Title
            };

            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
        }
        
        protected override bool OnBackButtonPressed() {
            return true;
        }

        protected override bool OnBackgroundClicked() {
            return base.OnBackgroundClicked();
        }

        private async void Open_Clicked(object sender, EventArgs e) {
            await PopupNavigation.RemovePageAsync(this);
            _messageBus.Send(new OpenBook { Book = _book });
        }

        private async void Delete_Clicked(object sender, EventArgs e) {
            var confirm = await DisplayActionSheet("Opravdu smazat knížku?", "Smazat", "Ne");
            if(confirm == "Smazat") {
                await PopupNavigation.RemovePageAsync(this);
                _messageBus.Send(new DeleteBook { Book = _book });
            }
        }
    }
}