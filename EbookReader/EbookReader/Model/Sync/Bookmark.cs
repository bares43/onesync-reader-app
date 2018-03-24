using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;
using Newtonsoft.Json;

namespace EbookReader.Model.Sync {
    public class Bookmark {
        [JsonIgnore]
        public long ID {
            get {
                return I;
            }
            set {
                I = value;
            }
        }

        [JsonIgnore]
        public bool Deleted {
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
                return new Position(S, P);
            }
            set {
                S = value.Spine;
                P = value.SpinePosition;
            }
        }

        [JsonIgnore]
        public string Name {
            get {
                return N;
            }
            set {
                N = value;
            }
        }

        [JsonIgnore]
        public DateTime LastChange {
            get {
                return C;
            }
            set {
                C = value;
            }
        } 

        public long I { get; set; }

        public bool D { get; set; }

        public int S { get; set; }

        public int P { get; set; }

        public string N { get; set; }

        public DateTime C { get; set; }
    }
}
