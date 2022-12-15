using GameClient.src.data;
using GameClient.src.networking;
using Newtonsoft.Json;
using ServerData.src.account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    internal class Account : IMenu
    {
        private List<Choice> choices;
        public int id;
        public List<string> Text { get; set; }

        public Account(int _id)
        {
            Text = new List<string>();
            id = _id;
            choices = new List<Choice>()
            {
                new Choice("GO BACK", () => MenuManager.UpdateCurrent(0))

            };
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
