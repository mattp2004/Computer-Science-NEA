using GameClient.src.data;
using GameClient.src.menus.impl;
using GameClient.src.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.menus
{
    public class MenuManager
    {
        public static bool FirstLaunch = true;
        public static bool MenuOn;
        public static List<IMenu> Menus;
        private static IMenu Current { get; set; }

        private static string title = @"
   ___                  _        ___                   
  / __|___ _ _  ___ ___| |___   / __|__ _ _ __  ___ ___
 | (__/ _ \ ' \(_-</ _ \ / -_) | (_ / _` | '  \/ -_|_-<
  \___\___/_||_/__/\___/_\___|  \___\__,_|_|_|_\___/__/";
        public static void Init()
        {
            Menus = new List<IMenu>();
            Menus.Add(new Validate(0));
            Menus.Add(new Main(1));
            Menus.Add(new Play(2));

            MenuOn = true;

            Current = Menus[0];
            if (DataManager.GetInstance().loggedIn) { Current = Menus[1]; }
            Menu();
        }

        public static void Menu()
        {
            int selected = 0;
            while (MenuOn)
            {
                Console.CursorVisible = false;
                Console.Clear();
                if (FirstLaunch) { Client.WriteLine(title); }
                else { Console.WriteLine(title); }
                for (int i = 0; i < Current.GetChoices().Count; i++)
                {
                    Console.WriteLine(); Console.WriteLine();
                    if (i == selected)
                    {
                        if (FirstLaunch)
                        {
                            Client.WriteLine($"  [ " + Current.GetChoices()[i].ChoiceName + " ] ");
                        }
                        else
                        {
                            Console.WriteLine($"  [ " + Current.GetChoices()[i].ChoiceName + " ] ");
                        }
                    }
                    else
                    {
                        if (FirstLaunch)
                        {
                            Client.WriteLine($"    " + Current.GetChoices()[i].ChoiceName);
                        }
                        else
                        {
                            Console.WriteLine($"    " + Current.GetChoices()[i].ChoiceName);
                        }
                    }
                    Console.WriteLine();
                }

                FirstLaunch = false;
                ConsoleKeyInfo key;
                key = Console.ReadKey();
                if (key.Key== ConsoleKey.Enter)
                {
                    Current.GetChoices()[selected].ChoiceExecute();
                    selected = 0;
                }
                else if (key.Key == ConsoleKey.DownArrow && selected < Current.GetChoices().Count-1)
                {
                    selected++;
                }
                else if (key.Key == ConsoleKey.UpArrow && selected > 0)
                {
                    selected--;
                }
            }
        }

        public static void UpdateCurrent(int id)
        {
            if(id == 0 && DataManager.GetInstance().loggedIn) { Current = Menus[1]; }
            Current = Menus[id];
        }
    }
}
