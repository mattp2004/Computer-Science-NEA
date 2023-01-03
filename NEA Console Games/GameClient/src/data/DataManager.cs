using GameClient.src.api;
using GameClient.src.menus;
using GameClient.src.Util;
using Newtonsoft.Json;
using ServerData.src.account;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public string Username;
        public AccountSet account;
        public string CustomGameCode;

        public DataManager()
        {
            instance = this;
            apiController = new ApiController();
            apiRepo = new ApiRepository(apiController);
            CustomGameCode = null;
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
                this.Username = username;
                accessToken = output;
                loggedIn = true;
                MenuManager.UpdateCurrent(1);
                Client.WriteLine("Successfully logged in.", ConsoleColor.Green, false);
                GetData();
                Client.Title = Client.Title + " | (" + username + ")";
                Client.SetTitle(Client.Title);
                Thread.Sleep(2500);
            }
        }

        public void Register()
        {
            string username = "";
            string password = "";
            bool valid = false;
            Client.WriteLine("Please enter a username between 3-12 characters");
            while(!valid){
                Client.WriteLine("USERNAME");
                username = Client.GetInput(); 
                if(username.Length > 3 && username.Length < 12)
                {
                    valid = true;
                }
                else
                {
                    Client.WriteLine("INVALID", ConsoleColor.Red, false);
                }
                if (apiRepo.AccountExists(username))
                {
                    valid = false;
                    Client.WriteLine("INVALID: USERNAME TAKEN.", ConsoleColor.Red, false);
                }
            }
            Client.WriteLine("Please enter a password between 6-18 characters");
            valid = false;
            while (!valid)
            {
                Console.WriteLine("PASSWORD");
                password = Client.GetInput();
                if (password.Length > 6 && password.Length < 18)
                {
                    valid = true;
                }
                else
                {
                    Client.WriteLine("INVALID", ConsoleColor.Red, false);
                }
            }
            apiRepo.CreateAccount(username, password);
        }

        public void GetData()
        {
            string accountInfo = DataManager.GetInstance().apiRepo.AccountView(DataManager.instance.Username);
            string t = accountInfo.Substring(1, accountInfo.Length - 2);
            t = t.Replace(@"\", System.Environment.NewLine);
            account = JsonConvert.DeserializeObject<AccountSet>(t);
            t = t.Replace('{', ' '); t = t.Replace(@"""", ""); t = t.Replace(System.Environment.NewLine, " "); t = t.Replace(",", System.Environment.NewLine); t = t.Replace(" :", ":");
            MenuManager.Menus[4].Text = new List<string>();
            MenuManager.Menus[4].Text.Add(t);

            string welcomeMSG = "";
            if (DateTime.Now.Hour <= 12)
            {
                welcomeMSG = ("Good Morning");
            }
            else if (DateTime.Now.Hour <= 16)
            {
                welcomeMSG = ("Good Afternoon");
            }
            else if (DateTime.Now.Hour <= 20)
            {
                welcomeMSG = ("Good Evening");
            }
            else
            {
                welcomeMSG = ("Good Night");
            }
            welcomeMSG += " " + Username;
            MenuManager.Menus[1].Text.Add(welcomeMSG);
        }

        public static DataManager GetInstance() { return instance; }
    }
}
