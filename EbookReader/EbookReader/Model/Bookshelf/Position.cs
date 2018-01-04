using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Bookshelf {
    public class Position {
        public int Spine { get; set; }
        public int SpinePosition { get; set; }

        public Position() {
        }

        public Position(Position position) {
            Spine = position.Spine;
            SpinePosition = position.SpinePosition;
        }

        public bool Equals(Position obj) {
            return obj != null && Spine == obj.Spine && SpinePosition == obj.SpinePosition;
        }
    }
}
