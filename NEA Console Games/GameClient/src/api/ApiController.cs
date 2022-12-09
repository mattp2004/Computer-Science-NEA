using Newtonsoft.Json;
using ServerData.src.account;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.api
{
    public class ApiController
    {
        public HttpClient httpClient;
        public ApiController()
        {
            httpClient = new HttpClient();
        }

        public string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), true);
                string strResponse = reader.ReadToEnd();
                return strResponse;
            }
            catch(Exception e) { }
            return "FAILED";
        }

        public void POST(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            request.Expect = "application/json";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), true);
                string strResponse = reader.ReadToEnd();
            }
            catch (Exception e) { }
        }

        public void POST(string url, Account a)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            request.Expect = "application/json";
            string json = JsonConvert.SerializeObject(a);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), true);
                string strResponse = reader.ReadToEnd();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
