using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace InventoryApp.Helpers
{
    public static class Settings
    {
        private static readonly string StringDefault = string.Empty;
        public static string Username
        {
            get { return Preferences. Get("user_name_key", StringDefault); }
            set { Preferences.Set("user_name_key", value); }
        }

        public static int CooperatorId
        {
            get { return Preferences.Get("cooperator_id_key", -1); }
            set { Preferences.Set("cooperator_id_key", value); }
        }

        public static string Token
        {
            get { return Preferences.Get("token_key", StringDefault); }
            set { Preferences.Set("token_key", value); }
        }

        public static int CooperatorGroupIndex
        {
            get { return Preferences.Get("CooperatorGroupIndex", -1); }
            set { Preferences.Set("CooperatorGroupIndex", value); }
        }

        public static string Location1
        {
            get { return Preferences.Get("Location1", StringDefault); }
            set { Preferences.Set("Location1", value); }
        }

        public static string Server
        {
            get { return Preferences.Get("Server", "genebank.cipotato.org"); }
            set { Preferences.Set("Server", value); }
        }
        public static int InventoryMaintPolicyId
        {
            get { return Preferences.Get("InventoryMaintPolicyId", -1); }
            set { Preferences.Set("InventoryMaintPolicyId", value); }
        }

        public static string Filter
        {
            get { return Preferences.Get("Filter", StringDefault); }
            set { Preferences.Set("Filter", value); }
        }
        public static string SearchText
        {
            get { return Preferences.Get("SearchText", StringDefault); }
            set { Preferences.Set("SearchText", value); }
        }



        public static string SearchAccessionSearchText
        {
            get { return Preferences.Get("SearchAccession_SearchText", StringDefault); }
            set { Preferences.Set("SearchAccession_SearchText", value); }
        }
        public static string SearchAccessionFilter
        {
            get { return Preferences.Get("SearchAccession_Filter", StringDefault); }
            set { Preferences.Set("SearchAccession_Filter", value); }
        }

        #region PrinterPage
        public static int PrinterIndex
        {
            get { return Preferences.Get("PrinterIndex", -1); }
            set { Preferences.Set("PrinterIndex", value); }
        }
        #endregion
    }

}
