using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.account
{
    class AccountRepository
    {
        private static string SELECT_ACCOUNT_BY_ID = "SELECT * FROM accounts WHERE id=?";
        private static string SELECT_RANK_BY_ID = "SELECT Rank FROM accounts WHERE id=?";


        private int accountID;
        public AccountRepository(int id)
        {
            accountID = id;
        }


        public Rank GetRank()
        {
            return Rank.OWNER;
        }

        public class ResultSet
        {
            public string username;
            public Rank rank;
            public int Points;
        }
    }
}
