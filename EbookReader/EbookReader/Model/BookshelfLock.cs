using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Model {
    public class BookshelfLock {

        static bool locked = false;

        public static bool Lock() {
            if (locked) return false;

            locked = true; ;
            Device.StartTimer(new TimeSpan(0, 0, 1), () => {
                locked = false;
                return false;
            });

            return true;
        }
    }
}
