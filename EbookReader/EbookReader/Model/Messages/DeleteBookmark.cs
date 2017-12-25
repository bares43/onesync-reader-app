using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class DeleteBookmark {
        public Model.Bookshelf.Bookmark Bookmark { get; set; }
    }
}
