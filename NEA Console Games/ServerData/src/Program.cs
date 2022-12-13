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
        }
    }
}
