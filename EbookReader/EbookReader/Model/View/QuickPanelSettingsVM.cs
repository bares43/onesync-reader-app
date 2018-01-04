using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Provider;

namespace EbookReader.Model.View {
    public class QuickPanelSettingsVM {

        public QuickPanelSettingsVM() {
            FontSize = new FontSizeVM();
            Margin = new MarginVM();
        }

        public FontSizeVM FontSize { get; set; }

        public MarginVM Margin { get; set; }

    }
}
