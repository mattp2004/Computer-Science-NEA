using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.network
{

    [Serializable]
    public class Packet
    {
        public string Content;
        public string Type;

        public Packet(string type, string content)
        {
            Content = content;
            Type = type;
        }

        public static Packet Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<Packet>(jsonString);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
