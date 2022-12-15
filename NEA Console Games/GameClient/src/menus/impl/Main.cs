using GameClient.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    class Main : IMenu
    {
        private List<Choice> choices;
        public int id;
        public List<string> Text { get; set; }

        public Main(int _id)
        {
            Text = new List<string>();
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("Play", () => MenuManager.UpdateCurrent(2)),
                new Choice("Options", () => MenuManager.UpdateCurrent(3)),
                new Choice("Account", () => AccountMenu()),
                new Choice("Quit", () => Environment.Exit(0)),
            };
        }


        public void AccountMenu()
        {
            DataManager.instance.GetData();
            Thread.Sleep(15);
            MenuManager.UpdateCurrent(4);
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
