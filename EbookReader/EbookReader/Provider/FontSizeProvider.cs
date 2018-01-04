using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public static class FontSizeProvider {
        public static List<int> Items {
            get {
                var min = 12;
                var max = 80;

                return Enumerable.Range(min, max - min + 1).Where(o => o % 2 == 0).ToList();
            }
        }
    }
}
