using GameServer.src.misc;
using GameServer.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.account
{
    class AccountManager
    {

        public static Account FetchAccount(Client _client)
        {
            if (Account.GetCache().ContainsKey(_client))
            {
                return Account.GetCache()[_client];
            }
            else
            {
                AccountRepository.LoadAccount(_client);
                try
                {
                    return Account.GetCache()[_client];
                }
                catch(Exception e)
                {
                    Util.Error(e);
                }
            }
            return null;
        }
    }
}
