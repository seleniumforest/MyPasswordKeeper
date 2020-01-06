using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyPasswordKeeper.DataStorage
{
    /// <summary>
    /// todo integration with google cloud
    /// </summary>
    class GoogleDriveStorage : IDataStorage
    {
        public async Task<(List<Identity> identities, bool success, string message)> Load(string password)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool success, string message)> Upload(MemoryStream data)
        {
            throw new NotImplementedException();
        }
    }
}
