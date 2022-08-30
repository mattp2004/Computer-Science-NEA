using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using GameServer.src;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server gameServer = new Server("Staging", 8000);
            gameServer.Run();
        }
    }
}
