using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public static class MarginProvider {
        public static List<int> Items {
            get {
                return new List<int> {
                    15,
                    30,
                    45,
                };
            }
        }
    }
}
