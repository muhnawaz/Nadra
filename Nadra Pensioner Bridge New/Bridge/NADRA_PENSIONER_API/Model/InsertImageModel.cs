using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NADRA_PENSIONER_API.Model
{
    public class InsertImageModel
    {
        

        public string imageData { get; set; }

        public string cnic { get; set; }

        public string ipaddress { get; set; }
    }

    public class UpdateNadraResultModel
    {

        public string cnic { get; set; }

        public string ipaddress { get; set; }

        public string VERIFYED { get; set; }

        public string NADRA { get; set; }


        
    }
}
