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
        Task<(List<Identity> identities, bool success, string message)> Load(string password);
        Task<(bool success, string message)> Upload(MemoryStream data);
    }
}
