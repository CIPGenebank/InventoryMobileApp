using InventoryApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Interfaces
{
    public interface IDataStoreService
    {
        Task<List<ILookup>> GetCodeValueList(string groupName);

        Task<bool> SyncCodeValueLookup();

        Task<List<ILookup>> GetAccessionLookupList(string accessionNumber);
        Task<bool> SyncAccessionLookup();
        
    }
}
