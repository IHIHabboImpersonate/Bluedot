#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal class PasswordStep : Step
    {
        internal PasswordStep(string title = "", string description = "")
        {
            Title = title;
            Description = description;
            Examples = new[] {"No examples!", "That would be a security risk for morons who might use them!"};
        }

        internal override object Run()
        {
            CoreManager.InstallerCore.Out.OverwritePageContents(
                ToString("No default! If you leave it blank the password is considered blank!"));

            StringBuilder password = new StringBuilder();

            Stack<Point> cursorHistory = new Stack<Point>();

            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key != ConsoleKey.Backspace)
                {
                    cursorHistory.Push(new Point(Console.CursorLeft, Console.CursorTop));
                    password.Append(key.KeyChar);
                    Console.Write('*');
                }
                else
                {
                    if (password.Length == 0)
                        continue;

                    password.Length--;

                    Point backCursor = cursorHistory.Pop();
                    Console.SetCursorPosition(backCursor.X, backCursor.Y);
                    Console.Write(' ');
                    Console.SetCursorPosition(backCursor.X, backCursor.Y);
                }
            }
            return password.ToString();
        }
    }
}