using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.network
{
    class Packet
    {

        public Packet(string _cmd, string _msg)
        {
            Message = _msg;
            Command = _cmd;
        }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        public override string ToString()
        {
            return string.Format(
                "[Packet:\n" +
                $"  Command=`{Command}`\n" +
                $"  Message=`{Message}`]");
        }

        public string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Packet ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Packet>(json);
        }

    }
}
