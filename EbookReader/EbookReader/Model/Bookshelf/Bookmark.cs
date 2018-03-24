using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EbookReader.Model.Bookshelf {
    public class Bookmark {
        [PrimaryKey]
        public long ID { get; set; }
        public bool Deleted { get; set; }
        public DateTime LastChange { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string BookID { get; set; }
        public int Spine { get; set; }
        public int SpinePosition { get; set; }

        [Ignore]
        public virtual Position Position {
            get {
                return new Position(Spine, SpinePosition);
            }
            set {
                Spine = value.Spine;
                SpinePosition = value.SpinePosition;
            }
        }
    }
}