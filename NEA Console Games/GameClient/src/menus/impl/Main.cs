using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    class Main : IMenu
    {
        private List<Choice> choices;
        public int id;

        public Main(int _id)
        {
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(1);
            choices = new List<Choice>()
            {
                new Choice("Play", () => MenuManager.UpdateCurrent(1)),
                new Choice("Friends", () => MenuManager.UpdateCurrent(1)),
                new Choice("Options", () => MenuManager.UpdateCurrent(1)),
                new Choice("Account", () => MenuManager.UpdateCurrent(1)),
                new Choice("Quit", () => Environment.Exit(0)),
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
