using ServerData.src.account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.network
{
    public class Client
    {
        public Client(TcpClient _client, string _uuid)
        {
            tcpClient = _client;
            uuid = _uuid;
        }

        public string uuid;
        public TcpClient tcpClient;

        public Account GetAccount()
        {
            return Account.Get(this);
        }
    }
}