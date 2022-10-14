using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers
{
    public class Student : Item
    {
        public string name;
    }

    public class Input
    {
        public string token;
        public Item args;
    }

    public class Item
    {
        public string input;
    }

    public class ValuesController : ApiController
    {

        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post(Input value)
        {
            if(value.token == Config.AccessToken)
            {
                Console.Write("CORRECT AUTH TOKEN");
                Console.Write(value.args.input);
            }
            else
            {
                Console.Write("INVALID AUTH");
            }
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
