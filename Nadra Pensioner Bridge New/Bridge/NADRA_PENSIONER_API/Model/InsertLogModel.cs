using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model
{
    public class InsertLogModel
    {
        public string user_id {get;set;}
        public string sys_user {get;set;} 
        public string sys_name {get;set;}
        public string sys_ip  { get; set; }
    }
}
