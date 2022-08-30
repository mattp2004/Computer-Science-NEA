using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.account
{
    class AccountManager
    {
        public static Dictionary<TcpClient, Account> _userAccounts = new Dictionary<TcpClient, Account>();

        public static void AddAccount(TcpClient client, Account account)
        {
            _userAccounts.Add(client, account);
        }

        public static void RemoveAccount(TcpClient client)
        {
            _userAccounts.Remove(client);
        }

        public static Account GetAccount(TcpClient client)
        {
            return _userAccounts[client];
        }

        public static void GivePoints(Account acc, int num)
        {
            acc.AddPoints(num);
        }
    }
}
