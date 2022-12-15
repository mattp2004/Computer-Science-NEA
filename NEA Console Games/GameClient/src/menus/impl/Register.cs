using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    public class Register : IMenu
    {
        private List<Choice> choices;
        public int id;

        public List<string> Text { get; set; }

        public Register(int _id)
        {
            Text = null;
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("Login", () => MenuManager.UpdateCurrent(1)),
                new Choice("Register", () => MenuManager.UpdateCurrent(1)),
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
