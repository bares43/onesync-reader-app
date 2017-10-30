using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;

namespace EbookReader.Service {
    public interface IBookshelfService {
        void AddBook(FileData file);
    }
}
