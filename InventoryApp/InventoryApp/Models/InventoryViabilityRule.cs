using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class InventoryViabilityRule
    {
        [JsonProperty("inventory_viability_rule_id")]
        public int InventoryViabilityRuleId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("substrata")]
        public string Substrata { get; set; }

        [JsonProperty("seeds_per_replicate")]
        public int SeedsPerReplicate { get; set; }

        [JsonProperty("number_of_replicates")]
        public int NumberOfReplicates { get; set; }

        [JsonProperty("requirements")]
        public string Requirements { get; set; }

        [JsonProperty("category_code")]
        public string CategoryCode { get; set; }

        [JsonProperty("temperature_range")]
        public string TemperatureRange { get; set; }

        [JsonProperty("count_regime_days")]
        public int? CountRegimeDays { get; set; }

        [JsonProperty("moisture")]
        public string Moisture { get; set; }

        [JsonProperty("prechill")]
        public string Prechill { get; set; }

        [JsonProperty("lighting")]
        public string Lighting { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
