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

        private void Init() {

            // ioc
            _bookshelfService = IocManager.Container.Resolve<IBookshelfService>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            var books = new Model.Bookshelf.Bookshelf {
                Books = new List<Model.Bookshelf.Book> {
                    new Model.Bookshelf.Book {
                        Title = "jedna"
                    },
                    new Model.Bookshelf.Book {
                        Title = "dva"
                    },
                    new Model.Bookshelf.Book {
                        Title = "tri"
                    },
                    new Model.Bookshelf.Book {
                        Title = "ctyri"
                    },
                    new Model.Bookshelf.Book {
                        Title = "pet"
                    },
                    new Model.Bookshelf.Book {
                        Title = "sest"
                    }
                }
            };
            
            var wrap = new WrapLayout {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            wrap.Children.Add(new AddBookCard());

            foreach(var book in books.Books) {
                wrap.Children.Add(new BookCard(book));
            }
            
            Title = "E-book Reader";

            _messageBus.Subscribe<AddBookClicked>(LoadButton_Clicked);

            Content = new ScrollView {
                Content = wrap
            };

            //Content = new StackLayout {
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    Orientation = StackOrientation.Horizontal,
            //    Children = {
            //        wrap
            //    }
            //};
        }

        private void LoadButton_Clicked(AddBookClicked msg) {
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