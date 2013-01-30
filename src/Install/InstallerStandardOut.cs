#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General internal License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General internal License for more details.
// 
// You should have received a copy of the GNU General internal License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Text;

#endregion

namespace Bluedot.HabboServer.Install
{
    internal class InstallerStandardOut
    {
        internal InstallerStandardOut SetCategoryTitle(string text)
        {
            int requiredPadding = Console.BufferWidth - text.Length;

            if ((requiredPadding & 1) == 1) // Is RequiredPadding odd?
            {
                text += " "; // Yes, make it even.
                requiredPadding--;
            }

            text =
                text.PadLeft(text.Length + requiredPadding/2).PadRight(Console.BufferWidth).PadRight(
                    Console.BufferWidth*2, '=');

            Console.SetCursorPosition(0, 0);
            Console.Write(text);
            return this;
        }

        internal InstallerStandardOut SetStep(byte current, byte total)
        {
            string text = current + "/" + total;

            Console.SetCursorPosition(0, 2);
            Console.Write(text.PadLeft(Console.BufferWidth));
            return this;
        }

        internal InstallerStandardOut SetStatus(string text, ConsoleColor foreground = ConsoleColor.Gray,
                                       ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            text = text.Length > Console.BufferWidth - 1
                       ? text.Substring(0, Console.BufferWidth - 1)
                       : text.PadRight(Console.BufferWidth - 1);

            Console.SetCursorPosition(0, Console.BufferHeight - 1);
            Console.Write(text);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }

        internal InstallerStandardOut ClearPage()
        {
            Console.SetCursorPosition(0, 6);

            StringBuilder blankness = new StringBuilder
                                          {
                                              Length = Console.BufferWidth*(Console.BufferHeight - 7)
                                          };

            Console.Write(blankness.ToString());
            Console.SetCursorPosition(0, 6);
            return this;
        }

        internal InstallerStandardOut SetPage(string contents)
        {
            ClearPage();
            Console.SetCursorPosition(0, 6);
            Console.Write(contents);
            return this;
        }
    }
}