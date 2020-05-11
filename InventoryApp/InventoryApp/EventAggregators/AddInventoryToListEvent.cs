using InventoryApp.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.EventAggregators
{
    public class AddInventoryToListEvent : PubSubEvent<InventoryThumbnail>
    {
    }
}
