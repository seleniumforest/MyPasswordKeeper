using MyPasswordKeeper.ArchiveWorker;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyPasswordKeeper.DataStorage
{
    interface IDataStorage 
    {
        Task<List<Identity>> Load(string password);
        Task<bool> Upload(MemoryStream data);
    }
}
