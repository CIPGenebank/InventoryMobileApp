using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class InventoryViability : BindableBase
    {
        [JsonProperty("inventory_viability_id")]
        public int InventoryViabilityId { get; set; }

        [JsonIgnore]
        private int _inventory_viability_rule_id;
        [JsonProperty("inventory_viability_rule_id")]
        public int InventoryViabilityRuleId
        {
            get { return _inventory_viability_rule_id; }
            set { SetProperty(ref _inventory_viability_rule_id, value); }
        }

        [JsonProperty("inventory_id")]
        public int InventoryId { get; set; }

        [JsonProperty("tested_date_code")]
        public string TestedDateCode { get; set; }

        [JsonIgnore]
        private DateTime _testedDate;
        [JsonProperty("tested_date")]
        public DateTime TestedDate
        {
            get { return _testedDate; }
            set { SetProperty(ref _testedDate, value); }
        }

        [JsonProperty("percent_normal")]
        public decimal? PercentNormal { get; set; }

        [JsonProperty("percent_abnormal")]
        public decimal? PercentAbnormal { get; set; }

        [JsonIgnore]
        private decimal? _percentViable;
        [JsonProperty("percent_viable")]
        public decimal? PercentViable
        {
            get { return _percentViable; }
            set { SetProperty(ref _percentViable, value); }
        }

        [JsonIgnore]
        private int? _totalTestedCount;
        [JsonProperty("total_tested_count")]
        public int? TotalTestedCount
        {
            get { return _totalTestedCount; }
            set { SetProperty(ref _totalTestedCount, value); }
        }

        [JsonIgnore]
        private int? _replicationCount;
        [JsonProperty("replication_count")]
        public int? ReplicationCount
        {
            get { return _replicationCount; }
            set { SetProperty(ref _replicationCount, value); }
        }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonIgnore]
        private string _inventory_viability_rule_name;
        [JsonProperty("inventory_viability_rule_name")]
        public string InventoryViabilityRuleName
        {
            get { return _inventory_viability_rule_name; }
            set { SetProperty(ref _inventory_viability_rule_name, value); }
        }

        [JsonIgnore]
        public string ShortTestedDate { get { return TestedDate.ToString("MM/dd/yyyy"); } }

        public InventoryViability()
        {
            PercentNormal = 0;
            PercentViable = 0;
            PercentAbnormal = 0;
        }
    }
}
