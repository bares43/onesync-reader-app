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
                var stackLayout = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                };

                var previewMode = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                };

                var editMode = new StackLayout {
                    IsVisible = false,
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                var deleteIcon = new Image {
                    WidthRequest = 26,
                    HeightRequest = 26,
                    Source = "delete.png",
                };

                var deleteIconTgr = new TapGestureRecognizer();
                deleteIconTgr.Tapped += (s, e) => this.DeleteBookmark(item);
                deleteIcon.GestureRecognizers.Add(deleteIconTgr);

                var entry = new Entry {
                    Text = item.Name,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                // preview
                var editIcon = new Image {
                    WidthRequest = 26,
                    HeightRequest = 26,
                    Source = "edit.png",
                };

                var editIconTgr = new TapGestureRecognizer();
                editIconTgr.Tapped += (s, e) => {
                    editMode.IsVisible = true;
                    previewMode.IsVisible = false;
                    entry.Focus();
                };
                editIcon.GestureRecognizers.Add(editIconTgr);

                var label = new Label {
                    Text = item.Name,
                    FontSize = Device.GetNamedSize(Device.RuntimePlatform == Device.Android ? NamedSize.Large : NamedSize.Medium, typeof(Label))
                };

                var tgr = new TapGestureRecognizer();
                tgr.Tapped += (s, e) => this.ClickToItem(item);
                label.GestureRecognizers.Add(tgr);

                previewMode.Children.Add(editIcon);
                previewMode.Children.Add(label);

                // edit
                var saveIcon = new Image {
                    WidthRequest = 26,
                    HeightRequest = 26,
                    Source = "save.png",
                };

                var saveIconTgr = new TapGestureRecognizer();
                saveIconTgr.Tapped += (s, e) => {
                    previewMode.IsVisible = true;
                    editMode.IsVisible = false;
                    item.Name = entry.Text;
                    IocManager.Container.Resolve<IMessageBus>().Send(new ChangedBookmarkName { Bookmark = item });
                };
                saveIcon.GestureRecognizers.Add(saveIconTgr);

                editMode.Children.Add(saveIcon);
                editMode.Children.Add(entry);

                stackLayout.Children.Add(deleteIcon);
                stackLayout.Children.Add(editMode);
                stackLayout.Children.Add(previewMode);

                layouts.Add(stackLayout);
            }

            return layouts;
        }

        private void ClickToItem(Model.Bookshelf.Bookmark item) {
            IocManager.Container.Resolve<IMessageBus>().Send(new OpenBookmark { Bookmark = item });
        }

        private void DeleteBookmark(Model.Bookshelf.Bookmark item) {
            IocManager.Container.Resolve<IMessageBus>().Send(new DeleteBookmark { Bookmark = item });
        }

    }
}