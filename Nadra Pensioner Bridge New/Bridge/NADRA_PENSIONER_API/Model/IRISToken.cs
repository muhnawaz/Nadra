using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model
{
    public class IRISToken
    {
    }

    public class IrisTokenResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
}
