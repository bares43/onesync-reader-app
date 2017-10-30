using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Page.Home {
    public class BookCard : Card {
        public BookCard(Model.Bookshelf.Book book) {

            var layout = new AbsoluteLayout() {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            var titleLabel = new Label { Text = book.Title, BackgroundColor = Color.Black, TextColor = Color.White };
            layout.Children.Add(titleLabel, new Rectangle(0, 270, 300, 30));

            Children.Add(layout);
        }
    }
}
