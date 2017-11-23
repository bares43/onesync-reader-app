using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Bookshelf {
    public class Book {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Cover { get; set; }
        public Position Position { get; set; }

        public Book() {
            Position = new Position();
        }

    }

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
