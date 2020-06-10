using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class CodeValue
    {
        [JsonProperty("value_member")]
        public string Code { get; set; }
        [JsonProperty("display_member")]
        public string Value { get; set; }

        public CodeValue(string code, string value)
        {
            Code = code;
            Value = value;
        }
    }

    public static class CodeValueFactory
    {
        static List<CodeValue> _methodList = new List<CodeValue> {
            new CodeValue( "2", "Regeneration"),
            new CodeValue( "3", "Distribution"),
            new CodeValue( "4", "Correct Errors (daily work)"),
            new CodeValue( "5", "Inventory Adjustments (annual activity) "),
            new CodeValue( "6", "Other"),
            new CodeValue( "7", "Safety Duplicates International"),
            new CodeValue( "8", "DNA for Conservation"),
            new CodeValue( "9", "Svalbar")
        };
        static List<CodeValue> _actionNameCodeList = new List<CodeValue>{
            new CodeValue( "INCREASE", "Increase"),
            new CodeValue( "DISCOUNT", "Discount")
        };

        public static List<CodeValue> MethodList { get { return _methodList; } }
        public static List<CodeValue> ActionNameCodeList { get { return _actionNameCodeList; } }

        public static List<string> GetUnits()
        {
            return new List<string> { "each", "grams", "milligrams", "Botanic seed", "Grams of seed", "Berry", "Plant" };
        }
        public static List<string> GetSearchInventoryFilters()
        {
            return new List<string> { "Accession Number", "Accession Name", "Inventory Id / Lot Id"
                , "Collecting number","Storage location level 2", "Storage location level 3", "Note", "Order request - Local number", "Order request - Order Id"
                /* "Distribution Request",*/ /*"List Name",*/ 
                /*"Accession ID", "Inventory Number",*/ };
        }
        public static List<string> GetSearchInventoryActivities()
        {
            return new List<string> { "Wild Potato", "Sweetpotato", "ARTC" };
        }
        public static List<string> GetSearchInventoryLocations()
        {
            return new List<string> { "Chamber A (0 C)", "Chamber B (-10 C)", "Chamber C (-20 C)" };
        }

        public static List<string> GetContainerTypeList()
        {
            return new List<string> { "Paper Envelope 8.6x5.8 cm", "Paper Envelope 18.2x10.2 cm", "Aluminium Envelope 8x12 cm", "Aluminium Envelope 21.5x17 cm", "Cardboard Folkote 29x42 cm", "Eppendorf" };
        }

        #region Printing
        public static List<string> GetPrinterList()
        {
            return new List<string> { "\\\\CIP0977\\Printer", "10.10.10.20", "10.10.10.30" };
        }
        public static List<string> GetLabelDesignList()
        {
            return new List<string> { "Seed Label", "ADN Label", "Herbarium Label" };
        }
        #endregion

        #region WelcomePage

        public static List<string> GetWorkgroupList()
        {
            return new List<string> { "Wild Potato", "Cultivated Potato", "Sweetpotato", "ARTC" };
        }
        public static List<string> GetLocationList()
        {
            return new List<string> { "Chamber A", "Chamber B", "Chamber C" };
        }

        #endregion
    }
}
