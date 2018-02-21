using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EbookReader.Service {
    public interface IDatabaseService {
        SQLiteAsyncConnection Connection { get; }
    }
}
