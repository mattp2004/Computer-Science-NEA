using GameServer.src.account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.network
{
    class Client
    {
        public Client(TcpClient _client, Account _account)
        {
            tcpClient = _client;
            account = _account;
        } 

        public Account account;
        public TcpClient tcpClient;
    }
}
