using ServerData.src.data;
using System;

namespace ServerData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataManager dataManager = new DataManager();
            dataManager.Run();
            Console.ReadKey();
            //api crashes when posted auth token maybe bc thread is cancelled or changed by the posting or auth repo not too sure??/

            //AccountSet accountSet = new AccountSet();
            //accountSet.uuid = $"{DataUtil.GenerateUUID()}";
            //accountSet.id = 100;
            //accountSet.username = "dcdb";
            //accountSet.password = "TestPassword34";
            //accountSet.tokens = 90000;
            //accountSet.created = DateTime.Now;
            //accountSet.rank = Rank.OWNER;
            //accountSet.lastjoined = DateTime.Now;

            //Client b = new Client(new System.Net.Sockets.TcpClient(), accountSet.uuid);
            //Account acc = new Account(accountSet, b);
            //dataManager.accountRepository.CreateAccount(acc);
            //Console.ReadKey();
            //while (true)
            //{
            //    dataManager.accountRepository.LoadAccount(b);
            //    Console.WriteLine(Account.GetCache()[b].ToString());
            //    Console.ReadKey();
            //}
        }
    }
}
