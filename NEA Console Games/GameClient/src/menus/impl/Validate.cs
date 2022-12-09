using GameClient.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    public class Validate : IMenu
    {
        private List<Choice> choices;
        public int id;

        public Validate(int _id)
        {
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("Login", () => DataManager.GetInstance().Login()),
                new Choice("Register", () => MenuManager.UpdateCurrent(0)),
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
