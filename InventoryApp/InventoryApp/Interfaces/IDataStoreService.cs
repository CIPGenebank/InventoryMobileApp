using InventoryApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Interfaces
{
    public interface IDataStoreService
    {
        List<ILookup> GetCodeValueList(string groupName);

        bool SyncCodeValueLookup();

        List<ILookup> GetAccessionLookupList(string accessionNumber);
        bool SyncAccessionLookup();
        
    }
}
