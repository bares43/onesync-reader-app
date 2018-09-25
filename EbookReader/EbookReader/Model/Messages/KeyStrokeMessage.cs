using System;
using System.Collections.Generic;
using System.Text;

namespace EbookReader.Model.Messages {
    public class KeyStrokeMessage {
        public Key? Key { get; set; }

        private KeyStrokeMessage(Key? key) {
            this.Key = key;
        }

        public static KeyStrokeMessage FromKeyCode(int keyCode) {
            Key? key = null;

            switch (keyCode) {
                case 32:
                    key = Messages.Key.Space;
                    break;
                case 37:
                    key = Messages.Key.ArrowLeft;
                    break;
                case 39:
                    key = Messages.Key.ArrowRight;
                    break;
                case 38:
                    key = Messages.Key.ArrowUp;
                    break;
                case 40:
                    key = Messages.Key.ArrowDown;
                    break;
            }

            return new KeyStrokeMessage(key);
        }
    }

    public enum Key {
        Space,
        ArrowLeft,
        ArrowRight,
        ArrowUp,
        ArrowDown,
    }
}
