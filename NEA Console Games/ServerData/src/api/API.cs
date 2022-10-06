using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.api
{
    internal class API
    {
        public API()
        {
            Run().Wait();
        }

        public async Task Run()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:19885/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync("api/person/3");
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("SUCCESS");
                }
            }
        }

        class Person
        {
        public long ID { get; set; }
            public string LastName { get; set; }
        
        }


    }
}
