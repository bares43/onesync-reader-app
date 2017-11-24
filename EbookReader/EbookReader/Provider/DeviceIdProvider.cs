using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public static class DeviceIdProvider {
        public static long ID {
            get {
                return (long)(DateTime.UtcNow - new DateTime(2017, 1, 1, 0, 0, 0)).TotalSeconds;
            }
        }
    }
}
