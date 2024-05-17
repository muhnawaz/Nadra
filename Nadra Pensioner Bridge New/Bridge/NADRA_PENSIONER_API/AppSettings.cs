using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 namespace NADRA_PENSIONER_API

{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ID { get; set; }
        public string PSW { get; set; }

        //for NADRA Integration
        public string username { get; set; }
        public string password { get; set; }
        public string IPAddress { get; set; }
        public string franchiseID { get; set; }
        public string userid { get; set; }
        public string pass { get; set; }

    }

    public class URLConfiguration
    {
        public string tokenURL { get; set; }
        public string apiBaseURL { get; set; }
        public string PINValidationURL { get; set; }
        public string PINChange { get; set; }
        public string CustomerDetails { get; set; }
        public string CustomerCards { get; set; }
        public string CardInfo { get; set; }
        public string CardStatus { get; set; }

        //for NADRA Integration
        public string URLforVerification { get; set; }
    }


}
