using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APS.WebAPI.Controllers
{
    public class sampleController : ApiController
    {
        // GET: api/sample
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/sample/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/sample
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/sample/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/sample/5
        public void Delete(int id)
        {
        }
    }
}
