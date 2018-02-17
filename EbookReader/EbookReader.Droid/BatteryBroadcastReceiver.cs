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
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;

namespace EbookReader.Droid {
    public class BatteryBroadcastReceiver : BroadcastReceiver {
        public override void OnReceive(Context context, Intent intent) {
            var messageBus = IocManager.Container.Resolve<IMessageBus>();
            messageBus.Send(new BatteryChange { });
        }
    }
}