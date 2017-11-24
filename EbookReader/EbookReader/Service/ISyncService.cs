using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using EbookReader.Model.Sync;

namespace EbookReader.Service {
    public interface ISyncService {
        void SaveProgress(string bookID, Position position);
        Task<Progress> LoadProgress(string bookID);
        void DeleteBook(string bookID);
    }
}
