using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.data
{
    public static class DataUtil
    {
        public static string Characters = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static Random rnd = new Random();

        public static int TokenLength = 8;

        public static string GenerateUUID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public static string GenerateToken()
        {
            string Token = "";
            for(int i = 0; i < TokenLength; i++)
            {
                Token += Characters[rnd.Next(0, Characters.Length)];
            }
            return Token;
        }
    }
}
