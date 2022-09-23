using Newtonsoft.Json;
using System;

namespace GameClient.src.networking
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