using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace InventoryApp.Helpers
{
    public static class Settings
    {
        private static readonly string StringDefaultValue = string.Empty;

        public static string ServerList
        {
            get { return Preferences.Get("server_list_key", StringDefaultValue); }
            set { Preferences.Set("server_list_key", value); }
        }
        public static string Server
        {
            get { return Preferences.Get("server_key", StringDefaultValue); }
            set { Preferences.Set("server_key", value); }
        }
        public static int LangId
        {
            get { return Preferences.Get("lang_id_key", -1); }
            set { Preferences.Set("lang_id_key", value); }
        }
        public static string Username
        {
            get { return Preferences.Get("user_name_key", StringDefaultValue); }
            set { Preferences.Set("user_name_key", value); }
        }
        public static int UserCooperatorId
        {
            get { return Preferences.Get("user_cooperator_id_key", -1); }
            set { Preferences.Set("user_cooperator_id_key", value); }
        }
        public static string UserToken
        {
            get { return Preferences.Get("token_key", StringDefaultValue); }
            set { Preferences.Set("token_key", value); }
        }

        public static string WorkgroupName
        {
            get { return Preferences.Get("workgroup_name_key", StringDefaultValue); }
            set { Preferences.Set("workgroup_name_key", value); }
        }
        public static string WorkgroupInvMaintPolicies
        {
            get { return Preferences.Get("workgroup_inv_maint_policies_key", StringDefaultValue); }
            set { Preferences.Set("workgroup_inv_maint_policies_key", value); }
        }
        public static string WorkgroupInventoryDataview
        {
            get {
                string value = Preferences.Get("workgroup_inventory_dataview_key", "");
                return !string.IsNullOrEmpty(value) ? value : "table_mob_inventory"; }
            set { Preferences.Set("workgroup_inventory_dataview_key", value); }
        }
        public static string WorkgroupInventoryThumbnailDataview
        {
            get { string value = Preferences.Get("workgroup_inventory_thumbnail_dataview_key", "");
                return !string.IsNullOrEmpty(value) ? value : "get_mob_inventory_thumbnail"; }
            set { Preferences.Set("workgroup_inventory_thumbnail_dataview_key", value); }
        }

        public static string Location1
        {
            get { return Preferences.Get("Location1", StringDefaultValue); }
            set { Preferences.Set("Location1", value); }
        }

        public static string Filter
        {
            get { return Preferences.Get("Filter", StringDefaultValue); }
            set { Preferences.Set("Filter", value); }
        }
        public static string SearchText
        {
            get { return Preferences.Get("SearchText", StringDefaultValue); }
            set { Preferences.Set("SearchText", value); }
        }

        public static string SearchAccessionSearchText
        {
            get { return Preferences.Get("SearchAccession_SearchText", StringDefaultValue); }
            set { Preferences.Set("SearchAccession_SearchText", value); }
        }
        public static string SearchAccessionFilter
        {
            get { return Preferences.Get("SearchAccession_Filter", StringDefaultValue); }
            set { Preferences.Set("SearchAccession_Filter", value); }
        }

        public static string Printer
        {
            get { return Preferences.Get("printer_key", StringDefaultValue); }
            set { Preferences.Set("printer_key", value); }
        }

    }

}
