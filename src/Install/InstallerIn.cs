#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#endregion

namespace IHI.Server.Install
{
    internal class InstallerIn
    {
        /// <summary>
        ///   Read an unrestricted string.
        /// </summary>
        /// <param name = "defaultValue">A value to return if the read string is blank.</param>
        internal string GetString(string defaultValue = "")
        {
            string input = Console.ReadLine();
            if (input.Length == 0)
                return defaultValue;
            return input;
        }

        /// <summary>
        ///   Read an unsigned short.
        ///   If the read line is not a valid ushort between Minimum and Maximum an exception of type Bluedot.HabboServer.Install.InputException is thrown.
        /// </summary>
        /// <param name = "defaultValue">A value to return if the read line is blank.</param>
        /// <param name = "minimum">The minimum value to accept.</param>
        /// <param name = "maximum">The maxumum value to accept.</param>
        internal ushort GetUshort(ushort defaultValue = 0, ushort minimum = ushort.MinValue, ushort maximum = ushort.MaxValue)
        {
            string input = Console.ReadLine();

            if (input.Length == 0)
                return defaultValue;

            ushort value;

            if (ushort.TryParse(input, out value))
            {
                if (value < minimum)
                {
                    throw new InputException("Given input is lower than mimimum value [ " + value + " < " + minimum +
                                             " ]");
                }
                if (value > maximum)
                {
                    throw new InputException("Given input is higher than maximum value [ " + value + " > " + maximum +
                                             " ]");
                }
                return value;
            }
            throw new InputException("Given input is could not be parsed as an unsigned short [ " + input + " ]");
        }

        /// <summary>
        ///   Read a byte.
        ///   If the read line is not a valid byte between Minimum and Maximum an exception of type Bluedot.HabboServer.Install.InputException is thrown.
        /// </summary>
        /// <param name = "defaultValue">A value to return if the read line is blank.</param>
        /// <param name = "minimum">The minimum value to accept.</param>
        /// <param name = "maximum">The maxumum value to accept.</param>
        internal byte GetByte(byte defaultValue = 0, byte minimum = byte.MinValue, byte maximum = byte.MaxValue)
        {
            string input = Console.ReadLine();

            if (input.Length == 0)
                return defaultValue;

            byte value;

            if (byte.TryParse(input, out value))
            {
                if (value < minimum)
                {
                    throw new InputException("Given input is lower than mimimum value [ " + value + " < " + minimum +
                                             " ]");
                }
                if (value > maximum)
                {
                    throw new InputException("Given input is higher than maximum value [ " + value + " > " + maximum +
                                             " ]");
                }
                return value;
            }
            throw new InputException("Given input is could not be parsed as a byte [ " + input + " ]");
        }

        /// <summary>
        ///   Read a signed int.
        ///   If the read line is not a valid int between Minimum and Maximum an exception of type Bluedot.HabboServer.Install.InputException is thrown.
        /// </summary>
        /// <param name = "defaultValue">A value to return if the read line is blank.</param>
        /// <param name = "minimum">The minimum value to accept.</param>
        /// <param name = "maximum">The maxumum value to accept.</param>
        internal int GetInt(int defaultValue = 0, int minimum = int.MinValue, int maximum = int.MaxValue)
        {
            string input = Console.ReadLine();

            if (input.Length == 0)
                return defaultValue;

            int value;

            if (int.TryParse(input, out value))
            {
                if (value < minimum)
                {
                    throw new InputException("Given input is lower than mimimum value [ " + value + " < " + minimum +
                                             " ]");
                }
                if (value > maximum)
                {
                    throw new InputException("Given input is higher than maximum value [ " + value + " > " + maximum +
                                             " ]");
                }
                return value;
            }
            throw new InputException("Given input is could not be parsed as a signed int [ " + input + " ]");
        }

        /// <summary>
        ///   Read a line but mask the password as it is typed.
        /// </summary>
        internal string GetPassword()
        {
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