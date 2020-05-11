using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class InventoryViabilityData : BindableBase
    {
        [JsonProperty("inventory_viability_data_id")]
        public int InventoryViabilityDataId { get; set; }

        [JsonProperty("inventory_viability_id")]
        public int InventoryViabilityId { get; set; }

        [JsonProperty("counter_cooperator_id")]
        public int? CounterCooperatorId { get; set; }

        [JsonProperty("replication_number")]
        public int ReplicationNumber { get; set; }

        [JsonProperty("count_number")]
        public int CountNumber { get; set; }

        [JsonIgnore]
        private DateTime _countDate;
        [JsonProperty("count_date")]
        public DateTime CountDate
        {
            get { return _countDate; }
            set { SetProperty(ref _countDate, value, () => RaisePropertyChanged(nameof(ShortDate))); }
        }

        private int _normalCount;
        [JsonProperty("normal_count")]
        public int NormalCount
        {
            get { return _normalCount; }
            set { SetProperty(ref _normalCount, value); }
        }

        private int _abnormalCount;
        [JsonProperty("abnormal_count")]
        public int AbnormalCount
        {
            get { return _abnormalCount; }
            set { SetProperty(ref _abnormalCount, value); }
        }

        [JsonProperty("replication_count")]
        public int? ReplicationCount { get; set; }

        [JsonIgnore]
        private string _note;
        [JsonProperty("note")]
        public string Note
        {
            get { return _note; }
            set { SetProperty(ref _note, value); }
        }

        [JsonIgnore]
        public string ShortDate { get { return CountDate.ToString("MM/dd/yyyy"); } }
    }
}
