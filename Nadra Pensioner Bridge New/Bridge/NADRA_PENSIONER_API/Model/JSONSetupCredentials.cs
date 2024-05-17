using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model
{
    public class JSONSetupCredentials
    {
        public List<JsonUserSetup> SOURCE { get; set; }
    }
    public class JsonUserSetup
    {
        public string ID { get; set; }
        public string PASSWORD { get; set; }
    }
}
