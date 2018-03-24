using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class BookmarksChangedMessage {
        public List<Bookshelf.Bookmark> Bookmarks { get; set; }
    }
}
