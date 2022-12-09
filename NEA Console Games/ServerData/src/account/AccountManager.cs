using ServerData.src.data;
using ServerData.src.misc;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.account
{
    public class AccountManager
    {

        public static Account FetchAccount(Client _client)
        {
            if (Account.GetCache().ContainsKey(_client))
            {
                return Account.GetCache()[_client];
            }
            else
            {
                DataManager.instance.accountRepository.LoadAccount(_client);
                try
                {
                    return Account.GetCache()[_client];
                }
                catch (Exception e)
                {
                    Util.Error(e);
                }
            }
            return null;
        }
    }
}
