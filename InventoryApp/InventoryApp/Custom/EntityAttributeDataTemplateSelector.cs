using InventoryApp.Models;
using System;
using Xamarin.Forms;

namespace InventoryApp.Custom
{
    public class EntityAttributeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        public DataTemplate DateTimeTemplate { get; set; }
        public DataTemplate DropdownTemplate { get; set; }
        public DataTemplate LookupPickerTemplate { get; set; }
        public DataTemplate CheckboxTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            EntityAttribute ea = (EntityAttribute)item;
            switch (ea.ControlType) //None, CheckBox, Date / Time  Control, Textbox(free-form), LookupPicker, Dropdown, Texbox(integer only), Textbox(decimal only)
            {
                case "STRING":
                    return TextTemplate;
                case "INTEGER":
                    return NumberTemplate;
                case "DECIMAL":
                    return NumberTemplate;
                case "DROPDOWN":
                    return DropdownTemplate;
                case "DATETIME":
                    return DateTimeTemplate;
                case "LOOKUPPICKER":
                    return LookupPickerTemplate;
                case "CHECKBOX":
                    return CheckboxTemplate;
                default:
                    return TextTemplate;
            }
            /*if (ea.Type == typeof(string))
            {
                return TextTemplate;
            }
            else if (ea.Type == typeof(decimal))
            {
                return NumberTemplate;
            }
            else if (ea.Type == typeof(int))
            {
                return NumberTemplate;
            }
            else if (ea.Type == typeof(DateTime))
            {
                return DateTimeTemplate;
            }
            else {
                return TextTemplate;
            }*/
        }
    }
}
