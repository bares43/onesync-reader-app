using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class DeleteBook {
        public Bookshelf.Book Book { get; set; }
    }
}
