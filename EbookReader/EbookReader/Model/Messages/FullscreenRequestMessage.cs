using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class FullscreenRequestMessage {

        public bool Fullscreen {
            get {
                return setFullscreen && UserSettings.Reader.Fullscreen;
            }
        }

        bool setFullscreen;

        public FullscreenRequestMessage(bool setFullscreen) {
            this.setFullscreen = setFullscreen;
        }
    }
}
