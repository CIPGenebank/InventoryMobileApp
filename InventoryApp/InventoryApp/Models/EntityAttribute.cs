using DryIoc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class EntityAttribute : BindableBase
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; } //DATETIME, DECIMAL, INTEGER, STRING
        //spublic Object Value { get; set; }
        private Object _value;
        public Object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
        public string ControlType { get; set; }  //None, CheckBox, Date / Time  Control, Textbox(free-form), Dropdown, LookupPicker, Texbox(number), Textbox(only lleter)
        public string ControlSource { get; set; } //Dropdonw or LookUp Source
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }

        private string _displayValue;
        public string DisplayValue
        {
            get { return _displayValue; }
            set { SetProperty(ref _displayValue, value); }
        }

        public List<string> ListValues { get; set; }
        private bool _IsPicker;
        public bool IsPicker
        {
            get { return _IsPicker; }
            set { SetProperty(ref _IsPicker, value); }
        }
        public string SecondValue { get; set; }
    }
}
