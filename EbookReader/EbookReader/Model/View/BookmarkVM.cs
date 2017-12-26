using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class BookmarkVM : BaseVM {
        public Bookshelf.Bookmark Bookmark { get; set; }

        bool _editMode;
        public bool EditMode {
            get { return _editMode; }
            set {
                _editMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenBookmarkCommand { get; set; }
        public ICommand DeleteBookmarkCommand { get; set; }
        public ICommand ShowEditCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public BookmarkVM(Bookshelf.Bookmark bookmark) {
            Bookmark = bookmark;

            OpenBookmarkCommand = new Command(OpenBookmark);
            DeleteBookmarkCommand = new Command(DeleteBookmark);
            ShowEditCommand = new Command((entry) => ShowEdit(entry));
            SaveCommand = new Command(ChangeName);
        }

        private void OpenBookmark() {
            IocManager.Container.Resolve<IMessageBus>().Send(new OpenBookmark { Bookmark = Bookmark });
        }

        private void DeleteBookmark() {
            IocManager.Container.Resolve<IMessageBus>().Send(new DeleteBookmark { Bookmark = Bookmark });
        }

        public void ShowEdit(object obj) {
            EditMode = true;
            if (obj is Entry entry) {
                entry.Focus();
            }
        }

        public void ChangeName() {
            IocManager.Container.Resolve<IMessageBus>().Send(new ChangedBookmarkName { Bookmark = Bookmark });
            EditMode = false;
        }
    }
}