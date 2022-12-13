using GameClient.src.api;
using GameClient.src.menus;
using GameClient.src.Util;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.src.data
{
    public class DataManager
    {
        public static DataManager instance;
        public ApiController apiController;
        public ApiRepository apiRepo;
        public bool loggedIn = false;
        public string accessToken;
        public Games GameSelected;

        public DataManager()
        {
            instance = this;
            apiController = new ApiController();
            apiRepo = new ApiRepository(apiController);
        }

        public void Login()
        {
            Client.WriteLine("USERNAME");
            string username = Client.GetInput();
            Client.WriteLine("PASSWORD");
            string password = Client.GetInput();
            string output = DataManager.GetInstance().apiRepo.Login(username, password);
            Client.WriteLine(output);
            if(output.Substring(1,output.Length-2) == "FAILED")
            {
                Client.WriteLine("Incorrect username or password.", ConsoleColor.Red, false);
                Thread.Sleep(2500);
                MenuManager.UpdateCurrent(0);
            }
            else
            {
                accessToken = output;
                loggedIn = true;
                MenuManager.UpdateCurrent(1);
                Client.WriteLine("Successfully logged in.", ConsoleColor.Green, false);
                Thread.Sleep(2500);
            }
        }

        public static DataManager GetInstance() { return instance; }
    }
}
