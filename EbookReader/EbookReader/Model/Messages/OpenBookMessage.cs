using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class OpenBookMessage {
        public Bookshelf.Book Book { get; set; }
    }
}
