using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public class ScrollSpeedProvider {
        public static List<int> Items {
            get {
                return new List<int>() {
                    0,
                    200,
                    500,
                };
            }
        }
    }
}
