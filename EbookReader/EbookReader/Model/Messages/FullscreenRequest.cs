using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class FullscreenRequest {

        public bool Fullscreen {
            get {
                return setFullscreen && UserSettings.Reader.Fullscreen;
            }
        }

        bool setFullscreen;

        public FullscreenRequest(bool setFullscreen) {
            this.setFullscreen = setFullscreen;
        }
    }
}
