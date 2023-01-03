using GameClient.src.data;
using GameClient.src.networking;
using GameClient.src.Util;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    class Play : IMenu
    {
        private List<Choice> choices;
        public int id;
        public List<string> Text { get; set; }

        public Play(int _id)
        {
            Text = null;
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("QUICK PLAY", () => NetworkManager.Play()),
                new Choice("RPS", () => SetGame(ServerData.src.data.Games.RPS)),
                new Choice("GUESS NUMBER", () => SetGame(ServerData.src.data.Games.GUESS)),
                new Choice("BLACKJACK", () => SetGame(ServerData.src.data.Games.BLACKJACK)),
                new Choice("CUSTOM GAME", () => SetGame(Client.GetInput())),
                new Choice("GO BACK", () => MenuManager.UpdateCurrent(0))

            };
        }
        public void SetGame(Games a)
        {
            DataManager.instance.GameSelected = a;
            NetworkManager.Play();
        }
        public void SetGame(string code)
        {
            DataManager.instance.CustomGameCode = code;
            NetworkManager.Play();
        }

        public List<Choice> GetChoices()
        {
            return choices;
        }

        public int GetID()
        {
            return id;
        }
    }
}
