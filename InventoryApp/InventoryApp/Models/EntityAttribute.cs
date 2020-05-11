using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class EntityAttribute
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; } //DATETIME, DECIMAL, INTEGER, STRING
        public Object Value { get; set; }
        public string ControlType { get; set; }  //None, CheckBox, Date / Time  Control, Textbox(free-form), Dropdown, LookupPicker, Texbox(number), Textbox(only lleter)
        public string ControlSource { get; set; } //Dropdonw or LookUp Source
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
    }
}
