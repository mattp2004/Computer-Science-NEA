using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.stats
{
    [Serializable]
    public class Stats
    {
        public Stats()
        {
            GameWins = new Dictionary<Games, int>();
            GameLosses = new Dictionary<Games, int>();
            GamesPlayed = new Dictionary<Games, int>();
        }

        public Dictionary<Games, int> GameWins;
        public Dictionary<Games, int> GameLosses;
        public Dictionary<Games, int> GamesPlayed;

        public static Stats Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<Stats>(jsonString);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
