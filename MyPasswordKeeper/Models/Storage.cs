using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyPasswordKeeper.Models
{
    public class Storage
    {
        public string PathToArchive { get; set; }
        public string Password { get; set; }
        public IEnumerable<Identity> Data { get; set; }
        public static string fileNameInArchive => "userdata";
    }
}
