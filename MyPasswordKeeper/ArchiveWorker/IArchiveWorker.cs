using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyPasswordKeeper.ArchiveWorker
{
    interface IArchiveWorker
    {
        Task<IEnumerable<Identity>> GetDataFromArchive(MemoryStream stream, string password);
        Task<MemoryStream> CreateArchive(string password, IEnumerable<Identity> identities);

    }
}
