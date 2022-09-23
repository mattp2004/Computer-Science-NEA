using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.networking
{
    class NetworkManager
    {
        public static void QuickPlay()
        {
            NetworkClient test = new NetworkClient("localhost", 6000);
            test.Connect();
        }
    }
}
