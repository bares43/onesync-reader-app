using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.DependencyService {
    public interface ICryptoService {
        string GetMd5(byte[] bytes);
    }
}
