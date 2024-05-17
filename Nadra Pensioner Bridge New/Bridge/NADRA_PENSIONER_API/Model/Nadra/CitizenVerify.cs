using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model.Nadra
{
    public class CitizenVerify
    {
        
    }

    public class CitizenVerifyRequest
    {
        public string CNIC { get; set; }
        public string IssueDate { get; set; }
        public string BirthDate { get; set; }
        public string Area { get; set; }

        
    }
    public class CitizenVerifyRequest1
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string session_id { get; set; }
        public string citizen_number { get; set; }
        public string name { get; set; }
        public string father_husband_name { get; set; }
        public string present_address { get; set; }
        public string permanent_address { get; set; }
        public string date_of_birth { get; set; }
        public string birth_place { get; set; }
        public string mother_name { get; set; }
        public string photograph { get; set; }
        public string expiry_date { get; set; }
        public string card_type { get; set; }

    }

    public class CitizenVerifyResponse
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string session_id { get; set; }
        public string citizen_number { get; set; }
        public string name { get; set; }
        public string father_husband_name { get; set; }
        public string present_address { get; set; }
        public string permanent_address { get; set; }
        public string date_of_birth { get; set; }
        public string birth_place { get; set; }
        public string mother_name { get; set; }
        public string photograph { get; set; }
        public string expiry_date { get; set; }
        public string card_type { get; set; }
    }
}
