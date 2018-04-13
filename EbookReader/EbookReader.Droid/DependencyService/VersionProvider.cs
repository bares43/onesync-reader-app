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
    public class VersionProvider : IVersionProvider {

        public string AppVersion {
            get {
                var context = Application.Context;

                var manager = context.PackageManager;
                var info = manager.GetPackageInfo(context.PackageName, 0);

                return info.VersionName;
            }
        }
    }
}