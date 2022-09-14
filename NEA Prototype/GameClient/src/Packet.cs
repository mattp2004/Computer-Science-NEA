using Newtonsoft.Json;
using System;

namespace GameClient.src
{
    public enum PacketType
    {
        auth,
        cmd,
        msg
    }
    [Serializable]
    public class Packet
    {
        public string Content;
        public PacketType Type;

        public Packet(PacketType type, string content)
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