using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Exceptions;
using EbookReader.Model.Format;
using EbookReader.Service;

namespace EbookReader.Helpers {
    public static class EbookFormatHelper {

        public static IBookLoader GetBookLoader(string filename) {
            if (!string.IsNullOrEmpty(filename)) {

                EbookFormat ebookFormat = EbookFormat.Epub;

                if (filename.EndsWith(".txt")) {
                    ebookFormat = EbookFormat.Txt;
                }
                
                return GetBookLoader(ebookFormat);
            }

            throw new UnknownFileFormatException(filename);
        }

        public static IBookLoader GetBookLoader(EbookFormat format) {
            return IocManager.Container.ResolveKeyed<IBookLoader>(format);
        }

    }
}
