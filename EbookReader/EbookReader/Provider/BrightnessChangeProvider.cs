using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public static class BrightnessChangeProvider {
        public static List<string> Items {
            get {
                return new List<string> {
                    BrightnessChange.Left.ToString(),
                    BrightnessChange.Right.ToString(),
                    BrightnessChange.None.ToString(),
                };
            }
        }
    }

    public enum BrightnessChange {
        Left,
        Right,
        None
    }
}
