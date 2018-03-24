using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EbookReader.DependencyService;

namespace EbookReader.Droid.DependencyService {
    public class BatteryProvider : IBatteryProvider {
        public int RemainingChargePercent {
            get {

                using (var filter = new IntentFilter(Intent.ActionBatteryChanged)) {
                    using (var battery = Application.Context.RegisterReceiver(null, filter)) {


                        var level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
                        var scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

                        return (int)Math.Floor(level * 100D / scale);
                    }
                }
            }
        }
    }
}