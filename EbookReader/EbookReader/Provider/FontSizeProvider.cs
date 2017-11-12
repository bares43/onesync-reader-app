using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public static class FontSizeProvider {
        public static List<int> Items {
            get {
                return new List<int> {
                    12,
                    14,
                    16,
                    18,
                    20,
                    22,
                    24,
                    26,
                    28,
                    30,
                    32,
                    34,
                    36,
                    38,
                    40
                };
            }
        }
    }
}
