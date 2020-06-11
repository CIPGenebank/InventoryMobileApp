using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class SearchFilter
    {
        public int filter_id { get; set; }
        public string filter_name { get; set; }
        public string filter_type { get; set; }
        public string filter_source { get; set; }
        public string filter_query { get; set; }
        public string filter_operators { get; set; }
        public string filter_by_list { get; set; }
        public string filter_is_pre_query { get; set; }
        public string filter_pre_query_table { get; set; }
    }
}
