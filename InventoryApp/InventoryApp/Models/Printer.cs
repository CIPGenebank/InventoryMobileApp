using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class Printer
    {
        [JsonProperty("printer_id")]
        public int PrinterId { get; set; }
        [JsonProperty("printer_name")]
        public string PrinterName { get; set; }
        [JsonProperty("printer_path")]
        public string PrinterPath { get; set; }
        [JsonProperty("printer_resolution")]
        public string PrinterResolution { get; set; }
        [JsonProperty("printer_method")]
        public string PrinterMethod { get; set; }

        /*
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public DateTime modified_date { get; set; }
        public int modified_by { get; set; }
        public DateTime owned_date { get; set; }
        public int owned_by { get; set; }
        */

        public override string ToString()
        {
            return PrinterId + "\n" + PrinterName + "\n" + PrinterPath + "\n" + PrinterResolution + "\n" + PrinterMethod;
        }
    }

    public static class PrinterFactory
    {
        public static List<Printer> GetPrinterList()
        {
            var printers = new List<Printer>
            {
                new Printer
                {
                    PrinterId = 59,
                    PrinterName = "Genebank Dev",
                    PrinterPath = @"\\cip0977\GenebankDev_ZM400.200dpi",
                    PrinterResolution = "L",
                    PrinterMethod = "Shared"
                },
                new Printer
                {
                    PrinterId = 54,
                    PrinterName = @"Papa silvestre - Grupo de Violeta",
                    PrinterPath = @"172.25.19.7",
                    PrinterResolution = "L",
                    PrinterMethod = "IP"
                },
                new Printer
                {
                    PrinterId = 40,
                    PrinterName = @"\\w7p-genb002\ZebraGWP",
                    PrinterPath = @"\\w7p-genb002\ZebraGWP",
                    PrinterResolution = "L",
                    PrinterMethod = "Shared"
                },
                new Printer
                {
                    PrinterId = 43,
                    PrinterName = "Impresora de RTAs en invernadero",
                    PrinterPath = @"\\cip0903\ZebraLab9",
                    PrinterResolution = "L",
                    PrinterMethod = "Shared"
                },
                new Printer
                {
                    PrinterId = 59,
                    PrinterName = "Banco de ADN",
                    PrinterPath = @"\\w7p-rrobles\BancoDeADN",
                    PrinterResolution = "H",
                    PrinterMethod = "Shared"
                },
                new Printer
                {
                    PrinterId = 59,
                    PrinterName = "Papa silvestre - Grupo de Violeta",
                    PrinterPath = @"172.25.19.7",
                    PrinterResolution = "L",
                    PrinterMethod = "IP"
                }
            };

            return printers;
        }
    }
}
