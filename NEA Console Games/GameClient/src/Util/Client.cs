using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.src.Util
{
    public static class Client
    {
        public static int messageTime = 0;
        public static ConsoleColor defaultColor = ConsoleColor.White;
        public static string Version = "0.1";

        public static void WriteLine(string message)
        {
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                Thread.Sleep((int)messageTime);
            }
            Console.WriteLine();
        }

        public static void WriteLine(string message, bool centered)
        {
            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
            Console.WriteLine(message);
        }

        public static void WriteLine(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            WriteLine(message);
            Console.ForegroundColor = defaultColor;
            Console.WriteLine();
        }

        public static void WriteLine(string message, ConsoleColor colour, bool centered)
        {
            Console.ForegroundColor = colour;
            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
            Console.WriteLine();
        }

        public static void SetTitle(string message)
        {
            Console.Title = message;
        }

        public static string GetInput()
        {
            Console.CursorVisible = true;
            WriteLine("INPUT: ");
            return Console.ReadLine();
            Console.CursorVisible = false;
        }
    }
}
