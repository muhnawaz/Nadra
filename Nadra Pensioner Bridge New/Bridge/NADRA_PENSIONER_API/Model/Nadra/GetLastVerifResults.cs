using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model.Nadra
{
    public class GetLastVerifResults
    {
        
    }
    public class GetLastVerifResultsRequest
    {
        public string CNIC { get; set; }

        public string transactionId { get; set; }

    }

    public class GetLastVerifResultsResponse
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string session_id { get; set; }
        public string citizen_number { get; set; }
        
    }
}
