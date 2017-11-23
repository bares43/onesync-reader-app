using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using Newtonsoft.Json;

namespace EbookReader.Model.Sync {
    public class Progress {

        [JsonIgnore]
        public string DeviceName {
            get {
                return D;
            }
            set {
                D = value;
            }
        }

        [JsonIgnore]
        public Position Position {
            get {
                return new Position { Spine = S, SpinePosition = P };
            }
            set {
                S = value.Spine;
                P = value.SpinePosition;
            }
        }

        public string D { get; set; }

        public int S { get; set; }

        public int P { get; set; }
    }
}
