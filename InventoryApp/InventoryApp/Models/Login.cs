using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class Login
    {
        //public int sys_user_id { get; set; }
        public string user_name { get; set; }
        //public char is_enabled { get; set; }
        public int? cooperator_id { get; set; }

        //public int sys_lang_id { get; set; }
        //public int site_id { get; set; }
        public string site { get; set; }
        //public string script_direction { get; set; }
        public string groups { get; set; }
        public string login_token { get; set; }
        public string warning { get; set; }

        public string Error { get; set; }
        public string Success { get; set; }
        public string Token { get; set; }
        public int? CooperatorId { get; set; }
    }
}
