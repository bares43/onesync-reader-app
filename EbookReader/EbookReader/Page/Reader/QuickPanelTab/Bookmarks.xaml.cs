using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Model.View;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Reader.QuickPanelTab {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bookmarks : StackLayout {
        public Bookmarks() {
            InitializeComponent();

            BindingContext = new BookmarksVM();
        }

        public void SetBookmarks(List<Model.Bookshelf.Bookmark> items) {
            Device.BeginInvokeOnMainThread(() => {
                this.SetItems(items);
            });
        }

        private void SetItems(List<Model.Bookshelf.Bookmark> items) {

            Items.Children.Clear();

            foreach (var item in this.GetItems(items)) {
                Items.Children.Add(item);
            }
        }

        private List<StackLayout> GetItems(List<Model.Bookshelf.Bookmark> items) {

            var layouts = new List<StackLayout>();

            foreach (var item in items.OrderBy(o => o.Position.Spine).ThenBy(o => o.Position.SpinePosition)) {
                layouts.Add(new BookmarksTab.Bookmark(item));
            }

            return layouts;
        }
    }
}