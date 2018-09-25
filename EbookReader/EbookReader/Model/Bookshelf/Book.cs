using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Format;
using SQLite;

namespace EbookReader.Model.Bookshelf {
    public class Book {
        [PrimaryKey]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Cover { get; set; }
        public DateTime? BookmarksSyncLastChange { get; set; }
        public EbookFormat Format { get; set; }
        public DateTime? FinishedReading { get; set; }

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