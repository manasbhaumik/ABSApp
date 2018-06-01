using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APS.BusinessEntity.Models.Common
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(".issued")]
        public string issued { get; set; }
        [JsonProperty(".expires")]
        public string expires { get; set; }
    }

    public class APIError
    {
        public string error { get; set; }
        public string error_description { get; set; }
    }
}
