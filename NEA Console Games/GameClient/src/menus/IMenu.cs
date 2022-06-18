using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus
{
    public class Choice
    {
        public Choice(string name, Action action)
        {
            ChoiceName = name; ChoiceExecute = action;
        }
        public string ChoiceName { get; }
        public Action ChoiceExecute { get; }

    }
    public interface IMenu
    {
        List<Choice> GetChoices();
        int GetID();
    }
}
