using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model
{
    public class Logs
    {
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public string request { get; set; }
        public DateTime request_date_time { get; set; }
        public string response { get; set; }
        public DateTime response_date_time { get; set; }
        public string status_code { get; set; }
        public string status_msg { get; set; }
        public string project_name { get; set; }
    }

    public class ImageLogs
    {
        public string ID { get; set; }
        public string CNIC { get; set; }
        public string VERIFYED { get; set; }
        public string APID { get; set; }
        public string IP_Address { get; set; }
        public DateTime request_date_time { get; set; }
        public DateTime response_date_time { get; set; }
        public string requester { get; set; }

        //No instrument Code
        public string request_type { get; set; }
        public string branch { get; set; }

        //No instrument Code
        public string instrument_code { get; set; }
        public string response { get; set; }
        public byte[] response_blob { get; set; }
    }
}
