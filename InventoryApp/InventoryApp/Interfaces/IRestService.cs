﻿using InventoryApp.Models;
using InventoryApp.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Interfaces
{
    interface IRestService
    {
        #region InventoryViability

        Task<List<InventoryViability>> RefreshInventoryViabilityAsync();
        Task SaveInventoryViabilityAsync(InventoryViability item, bool isNewItem);
        Task DeleteInventoryViabilityAsync(string id);

        Task<string> CreateInventoryViabilityAsync(InventoryViability item);
        Task<string> UpdateInventoryViabilityAsync(InventoryViability item);
        #endregion

        Task<List<InventoryThumbnail>> SearchInventoryThumbnailAsync(string query, string dataview, string resolver);

        Task<List<InventoryViability>> SearchInventoryViabilityAsync(string query, string dataview, string resolver);
        Task<List<InventoryViabilityRule>> SearchInventoryViabilityRuleAsync(string query, string dataview, string resolver);

        #region InventoryViability
        Task<List<InventoryViabilityData>> SearchInventoryViabilityDataAsync(string query, string dataview, string resolver);
        Task<string> CreateInventoryViabilityDataAsync(InventoryViabilityData item);
        Task<string> UpdateInventoryViabilityDataAsync(InventoryViabilityData item);
        #endregion

        Task<string> GetNewInventoryID();
        Task<string> UpdateInventoryAsync(Inventory item);
    }
}
