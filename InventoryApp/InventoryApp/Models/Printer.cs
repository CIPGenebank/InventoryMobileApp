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
        
        [JsonProperty("printer_uri")]
        public string PrinterUri { get; set; }
        
        [JsonProperty("printer_resolution")]
        public string PrinterResolution { get; set; }

        [JsonProperty("printer_connection_type")]
        public string PrinterConnectionType { get; set; }

        public override string ToString()
        {
            return PrinterId + "\n" + PrinterName + "\n" + PrinterUri + "\n" + PrinterResolution + "\n" + PrinterConnectionType;
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
                    PrinterUri = @"\\cip0977\GenebankDev_ZM400.200dpi",
                    PrinterResolution = "L",
                    PrinterConnectionType = "Shared"
                },
                new Printer
                {
                    PrinterId = 54,
                    PrinterName = @"Papa silvestre - Grupo de Violeta",
                    PrinterUri = @"172.25.19.7",
                    PrinterResolution = "L",
                    PrinterConnectionType = "IP"
                },
                new Printer
                {
                    PrinterId = 40,
                    PrinterName = @"\\w7p-genb002\ZebraGWP",
                    PrinterUri = @"\\w7p-genb002\ZebraGWP",
                    PrinterResolution = "L",
                    PrinterConnectionType = "Shared"
                },
                new Printer
                {
                    PrinterId = 43,
                    PrinterName = "Impresora de RTAs en invernadero",
                    PrinterUri = @"\\cip0903\ZebraLab9",
                    PrinterResolution = "L",
                    PrinterConnectionType = "Shared"
                },
                new Printer
                {
                    PrinterId = 59,
                    PrinterName = "Banco de ADN",
                    PrinterUri = @"\\w7p-rrobles\BancoDeADN",
                    PrinterResolution = "H",
                    PrinterConnectionType = "Shared"
                },
                new Printer
                {
                    PrinterId = 59,
                    PrinterName = "Papa silvestre - Grupo de Violeta",
                    PrinterUri = @"172.25.19.7",
                    PrinterResolution = "L",
                    PrinterConnectionType = "IP"
                }
            };

            return printers;
        }
    }
}
