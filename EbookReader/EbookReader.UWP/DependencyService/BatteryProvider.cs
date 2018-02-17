using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.DependencyService;

namespace EbookReader.UWP.DependencyService {
    public class BatteryProvider : IBatteryProvider {
        public int RemainingChargePercent => 100;
    }
}
