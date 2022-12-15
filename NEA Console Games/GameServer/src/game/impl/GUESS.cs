using GameServer.src.network;
using ServerData.src.data;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.src.game.impl
{
    class GUESS : IGame
    {
        Random rng;
        Server serverInstance;
        Client Player;
        Status GameStatus;
        //Properties

        #region properties
        public string GameName
        {
            get { return "Guess the number"; }
        }
        public int MaxPlayers
        {
            get { return 1; }
        }
        public int RequiredPlayers
        {
            get { return 1; }
        }
        #endregion

        public GUESS(Server server)
        {
            serverInstance = server;
            rng = new Random();
            GameStatus = Status.STOPPED;
        }

        public bool AddPlayer(Client player)
        {
            Player = player;
            return false;
        }

        public void DisconnectClient(Client client)
        {
            Player = null;
        }

        public void Start()
        {
            Server.SendMessage(Player, "Welcome to Guess the number.\n To win you must guess the number using the given clues.");
            Thread.Sleep(35);

            bool numGuessed = false;
            int guess;
            int answer = rng.Next(1, 100);

            GameStatus = Status.RUNNING;

            //Gets player inputs
            while (!numGuessed)
            {
                guess = -1;
                while (guess == -1) {
                    int.TryParse(Server.RequestInput(Player, "Guess a number").GetAwaiter().GetResult(), out guess);
                    if (guess == -1) { Server.SendMessage(Player, "Invalid num"); }
                }
                if(guess == answer)
                {
                    Server.SendMessage(Player, "You guessed the number!");
                    numGuessed = true;
                    GameStatus = Status.STOPPED; 
                }
                if(guess > answer)
                {
                    Server.SendMessage(Player, "The answer is lower!");
                }
                else if (guess < answer)
                {
                    Server.SendMessage(Player, "The answer is bigger!");
                }
            }
            serverInstance.accountRepository.GiveTokens(Player.GetAccount(), 250);
            Server.SendMessage(Player, "You have been awarded 250 tokens for playing!");
            Console.WriteLine("Game ending");
            serverInstance.Queue[ServerData.src.data.Games.GUESS].Clear();
            serverInstance.GameTypes.Remove(Games.GUESS);
            serverInstance.GameTypes.Add(Games.GUESS, new GUESS(serverInstance));
            Thread.Sleep(9500);
            serverInstance.DisconnectClient(Player, "Game ended");
            Thread.Sleep(1000);
        }
    }
}
