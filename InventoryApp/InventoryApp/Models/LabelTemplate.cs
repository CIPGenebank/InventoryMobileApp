using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class LabelTemplate
    {
        public string TemplateName { get; set; }
        public float PaperWidth { get; set; }
        public float PaperHeight { get; set; }
        public float HorizontalGap { get; set; }
        public float VerticalGap { get; set; }
        public int HorizontalCount { get; set; }
        public float MarginLeft { get; set; }
        public float MarginTop { get; set; }

        public int Density { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Zpl { get; set; }
        public string Dataview { get; set; }
    }
}
