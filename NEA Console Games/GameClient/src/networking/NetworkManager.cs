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
            NetworkClient test = new NetworkClient("38.242.132.154", 6000);
            test.ConnectToServer();
        }
    }
}
