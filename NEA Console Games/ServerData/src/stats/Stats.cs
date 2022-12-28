using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.stats
{
    public class Stats
    {
        public Dictionary<Games, int> GameWins;
        public Dictionary<Games, int> GameLosses;
        public Dictionary<Games, int> GamesPlayed;
    }
}
