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
using System.IO;
using System.Text;
using System.Threading;

#endregion

namespace Bluedot.HabboServer
{
    internal class StandardOut
    {
        private bool _hidden;
        internal bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                if (_hidden == value)
                    return;

                _hidden = value;
                if (!value)
                    PrintHistroy();
            }
        }

        /// <summary>
        ///   The past message colours.
        /// </summary>
        private ConsoleColor[] _historyColours;

        /// <summary>
        ///   The past header text.
        /// </summary>
        private string[] _historyHeaders;

        /// <summary>
        ///   The past message text.
        /// </summary>
        private string[] _historyMessages;

        /// <summary>
        ///   The past message timestamps.
        /// </summary>
        private DateTime?[] _historyTimestamps;

        private StandardOutImportance _importance;
        internal StandardOutImportance Importance
        {
            get
            {
                return _importance;
            }
            set
            {
                if (_importance == value)
                    return;
                Raw("IMPORTANT", "StandardOut Importance Changed [ " + _importance + " -> " + value + " ]", ConsoleColor.Yellow);
                _importance = value;
            }
        }

        /// <summary>
        ///   The last index of the rolling history arrays that was written to.
        /// </summary>
        private int _lastIndexWritten;

        internal StandardOut()
        {
            _historyHeaders = new string[Console.BufferHeight];
            _historyMessages = new string[Console.BufferHeight];
            _historyColours = new ConsoleColor[Console.BufferHeight];
            _historyTimestamps = new DateTime?[Console.BufferHeight];
            _lastIndexWritten = 0;
        }

        /// <summary>
        ///   Output a debug message.
        /// </summary>
        /// <param name = "message">The message to output.</param>
        internal StandardOut PrintDebug(string message)
        {
            if (Importance <= StandardOutImportance.Debug)
                Raw("DEBUG", message, ConsoleColor.White);
            return this;
        }

        /// <summary>
        ///   Output a warning message.
        /// </summary>
        /// <param name = "message">The message to output.</param>
        internal StandardOut PrintWarning(string message)
        {
            if (Importance <= StandardOutImportance.Warning)
                Raw("WARNING", message, ConsoleColor.DarkYellow);
            return this;
        }

        /// <summary>
        ///   Output an error message.
        /// </summary>
        /// <param name = "message">The message to output.</param>
        internal StandardOut PrintError(string message)
        {
            if (Importance <= StandardOutImportance.Error)
                Raw("ERROR", message, ConsoleColor.Red);
            return this;
        }

        /// <summary>
        ///   Output an exception in a formatted manner.
        /// </summary>
        /// <param name = "e">The exception to output.</param>
        /// <param name="saveException">If false then the exception will not be saved to file.</param>
        internal StandardOut PrintException(Exception e, bool saveException = true)
        {
            PrintError("Exception => " + e.GetType().FullName);
            PrintError("             " + e.Message);
            if(saveException)
                PrintError("             Saved to " + SaveExceptionToFile(e));
            return this;
        }

        /// <summary>
        ///   Output a general message.
        ///   Use this for most things.
        /// </summary>
        /// <param name = "message">The message to output.</param>
        internal StandardOut PrintNotice(string message)
        {
            if (Importance <= StandardOutImportance.Notice)
                Raw("NOTICE", message, ConsoleColor.Gray);
            return this;
        }

        /// <summary>
        ///   Output an important message.
        ///   Use when the message is important but not debugging, a warning or an error.
        /// </summary>
        /// <param name = "message">The message to output.</param>
        internal StandardOut PrintImportant(string message)
        {
            if (Importance <= StandardOutImportance.Important)
                Raw("IMPORTANT", message, ConsoleColor.Green);
            return this;
        }

        /// <summary>
        ///   Clear the output.
        /// </summary>
        internal StandardOut Clear()
        {
            lock (this)
            {
                Console.Clear();
                _historyHeaders = new string[Console.BufferHeight];
                _historyMessages = new string[Console.BufferHeight];
                _historyColours = new ConsoleColor[Console.BufferHeight];
                _historyTimestamps = new DateTime?[Console.BufferHeight];
                _lastIndexWritten = 0;
                return this;
            }
        }
        
        private void Raw(string header, string message, ConsoleColor colour, bool record = true,
                         DateTime? timestamp = null)
        {
            timestamp = timestamp ?? DateTime.Now;

            if (record)
                PushHistroy(header, message, colour, timestamp.Value);
            if (Hidden)
                return;
            if (String.IsNullOrEmpty(header))
            {
                Console.WriteLine();
                return;
            }

            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(timestamp.Value.ToLongTimeString() + " >> [" + header + "] ");
                Console.ForegroundColor = colour;
                Console.WriteLine(message);
            }
        }

        private void PushHistroy(string header, string message, ConsoleColor colour, DateTime timestamp)
        {
            lock (_historyHeaders)
            {
                if (_lastIndexWritten < Console.BufferHeight - 1)
                    _lastIndexWritten++;
                else
                    _lastIndexWritten = 0;

                _historyHeaders[_lastIndexWritten] = header;
                _historyMessages[_lastIndexWritten] = message;
                _historyColours[_lastIndexWritten] = colour;
                _historyTimestamps[_lastIndexWritten] = timestamp;
            }
        }

        private void PrintHistroy()
        {
            lock (_historyHeaders)
            {
                Console.Clear();
                for (int i = _lastIndexWritten + 1; i < _historyHeaders.Length; i++)
                {
                    Raw(_historyHeaders[i], _historyMessages[i], _historyColours[i], false);
                }
                for (int i = 0; i <= _lastIndexWritten; i++)
                {
                    Raw(_historyHeaders[i], _historyMessages[i], _historyColours[i], false);
                }
            }
        }

        /// <summary>
        ///   Check if the output is hidden from the screen or not.
        /// </summary>
        /// <returns>True if the output is hidden, false otherwise.</returns>
        internal StandardOutImportance GetImportance()
        {
            return Importance;
        }

        /// <summary>
        ///   Set whether the output is hidden from the screen or not.
        /// </summary>
        /// <returns>True to hide the output, false to show it.</returns>
        internal void SetImportance(StandardOutImportance importance)
        {
            if (Importance == importance) return;
            Raw("IMPORTANT", "StandardOut Importance Changed [ " + Importance + " -> " + importance + " ]",
                ConsoleColor.Yellow);
            Importance = importance;
        }


        /// <summary>
        /// Save data about an exception to file.
        /// </summary>
        /// <param name="exception">The exception to save.</param>
        /// <returns>The path the file was saved at.</returns>
        /// <remarks>
        /// Static due to the fact that this has to work without throwing an exception (possible recursive loop).
        /// Having it as a instance method adds more complexity and more to go wrong (such as NullReferenceExceptions).
        /// </remarks>
        private static string SaveExceptionToFile(Exception exception)
        {
            StringBuilder logText = new StringBuilder("IHIEXCEPTION\x01");
            logText.Append("TIME\x02" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "\x01");

            logText.Append("EXCEPTION-THREAD\x02" + Thread.CurrentThread.Name + "\x01");

            int i = 0;
            while (exception != null)
            {
                logText.Append("EXCEPTION[" + i + "]-TYPE\x02" + exception.GetType().FullName + "\x01");
                logText.Append("EXCEPTION[" + i + "]-MESSAGE\x02" + exception.Message + "\x01");
                logText.Append("EXCEPTION[" + i + "]-STACKTRACE\x02" + exception.StackTrace + "\x01");

                i++;
                exception = exception.InnerException;
            }

            string path = Path.Combine(Environment.CurrentDirectory, "dumps",
                                       "exception-" + DateTime.UtcNow.Ticks + ".ihidump");

            File.WriteAllText(path, logText.ToString());
            return path;
        }
    }


    internal enum StandardOutImportance
    {
        Debug,
        Notice,
        Important,
        Warning,
        Error
    }
}