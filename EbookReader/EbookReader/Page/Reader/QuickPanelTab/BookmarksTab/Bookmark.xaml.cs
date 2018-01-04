using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Reader.QuickPanelTab.BookmarksTab {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bookmark : StackLayout {
        public Bookmark(Model.Bookshelf.Bookmark bookmark) {
            InitializeComponent();

            BindingContext = new Model.View.BookmarkVM(bookmark);
        }
    }
}