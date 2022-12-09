using GameClient.src.networking;
using GameClient.src.Util;
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

        public Play(int _id)
        {
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("QUICK PLAY", () => NetworkManager.QuickPlay()),
                new Choice("CREATE LOBBY", () => MenuManager.UpdateCurrent(0)),
                new Choice("JOIN LOBBY", () => MenuManager.UpdateCurrent(0)),
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
