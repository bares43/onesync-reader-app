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
    public class ToastService : IToastService {
        public void Show(string message) {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}