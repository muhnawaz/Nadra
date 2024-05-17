using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model.Nadra
{
    public class FingerprintVerify
    {
        
    }
    public class FingerprintVerifyRequest
    {
        public string SESSION_ID { get; set; }
        public string CITIZEN_NUMBER { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string FINGER_INDEX { get; set; }
        public string FINGER_TEMPLATE { get; set; } 
        public string AREA_NAME { get; set; }
        public string ACCOUNT_TYPE { get; set; }

        public string CNIC { get; set; }
        public string ContactNumber { get; set; }

        public string FingerprintIndex { get; set; }

        public string FPTemplate { get; set; }
        public string Area { get; set; }


    }

    public class FingerprintVerifyResponse
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string session_id { get; set; }
        public string citizen_number { get; set; }
        public string finger_index { get; set; }

        //public string[] finger_index { get; set; }

        public string result { get; set; }


    }

    public class finger_index1
    {
        public string finger { get; set; }
    }

    public class Response 
    {
        public List<Responsedata> RESPONSE_DATA { get; set; }
    }
    public class Responsedata 
    {
        public List<responsestatus> RESPONSE_STATUS { get; set; }
        public string SESSION_ID { get; set; }
        public string CITIZEN_NUMBER { get; set; }
        public List<FingerIndex> FINGER_INDEX { get; set; }
    }
    public class responsestatus
    {
        public string CODE { get; set; }
        public string MESSAGE { get; set; }
    }
    public class FingerIndex 
    {
        public string[] FINGER { get; set; }
    }
}
