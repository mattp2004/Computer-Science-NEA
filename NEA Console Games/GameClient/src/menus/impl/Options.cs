using GameClient.src.networking;
using GameClient.src.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus.impl
{
    public class Options : IMenu
    {
        private List<Choice> choices { get; set; }
        public int id;
        public List<string> Text { get; set; }
        int count = 0;
        int colorNum = 1;
        ConsoleColor[] colours = { ConsoleColor.White, ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta};
        public bool Anim;

        public Options(int _id)
        {
            Text = new List<string>();
            id = _id;
            Action a = () => MenuManager.UpdateCurrent(id);
            choices = new List<Choice>()
            {
                new Choice("SLOW TEXT", () => SlowText()),
                new Choice("ANIMATIONS", () => Animations()),
                new Choice("TEXT COLOUR", () => TextColour()),
                new Choice("GO BACK", () => MenuManager.UpdateCurrent(0))

            };
        }

        private void Animations()
        {
            Client.Animations = !Client.Animations;
            Text.Add("Animations toggled: " + Client.Animations);
        }

        public void TextColour()
        {
            if (colorNum < colours.Length - 1)
            {
                colorNum += 1;
            }
            else
            {
                colorNum = 0;
            }
            Client.defaultColor = colours[colorNum];
            Text.Add("Text colour changed to: " + colours[colorNum].ToString());
        }

        public async void SlowText()
        {
            bool valid = false;
            int time = Client.messageTime;
            while (!valid)
            {
                string input = Client.GetInput();
                try
                {
                    time = int.Parse(input);
                    if (time < 60 && time > -1) { valid = true; }
                    else
                    {
                        valid = false;

                    }
                }
                catch (Exception e)
                {
                    valid = false;
                    Client.WriteLine($"INPUT ERROR: {e.Message}", ConsoleColor.Red);
                }

            }
            Client.messageTime = time;
            Text.Add("Message Time for Slow Text set to : " + Client.messageTime);
        }

        public List<Choice> GetChoices()
        {
            if(count > 3)
            {
                Text.Clear();
                count = 0;
            }
            else
            {
                count += 1;
            }
            return choices;
        }

        public int GetID()
        {
            return id;
        }
    }
}
