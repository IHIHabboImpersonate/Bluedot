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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace Bluedot.HabboServer
{
    internal static class EntryPoint
    {
        internal static void Main(string[] arguments)
        {
            Thread.CurrentThread.Name = "BLUEDOT-EntryThread";

            AppDomain.CurrentDomain.AssemblyResolve += LoadPackagedReferences;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;


            string configFile = "config.xml";

            Regex nameValueRegex = new Regex("^--(?<name>[\\w-]+)=(?<value>.+)$");

            foreach (string argument in arguments)
            {
                Match nameValueMatch = nameValueRegex.Match(argument);
                string name = nameValueMatch.Groups["name"].Value;
                string value = nameValueMatch.Groups["value"].Value;

                switch (name)
                {
                    case "config-file":
                        {
                            configFile = value;
                            break;
                        }
                }
            }

            Console.Title = "Bluedot Habbo Server";
            Console.WriteLine("Bluedot Habbo Server");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            CoreManager.InitialiseInstallerCore();
            CoreManager.InitialiseServerCore();

            // Reassign CTRL + C to safely shutdown.
            // CTRL + Break is still unsafe.
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += ShutdownKey;

            CoreManager.ServerCore.Boot(Path.Combine(Environment.CurrentDirectory, configFile));
        }

        private static Assembly LoadPackagedReferences(object sender, ResolveEventArgs args)
        {
            return LoadPackagedDll(new AssemblyName(args.Name).Name);
        }
        public static Assembly LoadPackagedDll(string name)
        {
            String resourceName = "Bluedot.HabboServer.Reference_Packaging." + name + ".dll";

            using (Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.IsTerminating)
                return;

            if (CoreManager.ServerCore != null)
                if (CoreManager.ServerCore.StandardOut != null)
                    CoreManager.ServerCore.StandardOut.Hidden = true;


            Console.WindowWidth = Console.BufferWidth = Console.WindowWidth * 2;

            Exception exception = e.ExceptionObject as Exception;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine("[===[ BLUEDOT STOP ERROR ]===]");
            Console.WriteLine("An unhandled exception has caused BLuedot to close!");
            Console.WriteLine("Details of the exception are below.");
            Console.WriteLine();
            Console.WriteLine("Time (UTC): " + DateTime.UtcNow);

            Console.WriteLine("Loaded Assemblies: ");
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Assembly.GetCallingAssembly() == assembly)
                    Console.WriteLine("!! " + assembly.FullName);
                else
                    Console.WriteLine("   " + assembly.FullName);
            }

            Console.WriteLine();
            Console.WriteLine("Exception Assembly: " + Assembly.GetCallingAssembly().FullName);
            Console.WriteLine("Exception Thread: " + Thread.CurrentThread.Name);
            Console.WriteLine("Exception Type: " + exception.GetType().FullName);
            Console.WriteLine("Exception Message: " + exception.Message);

            Console.Write("Has Inner Exception: ");
            Console.WriteLine(exception.InnerException == null ? "NO" : "YES");

            Console.WriteLine("Stack Trace:");
            Console.WriteLine("  " + exception.StackTrace.Replace(Environment.NewLine, Environment.NewLine + "  "));

            string logText = "IHISTOPERROR\x01";
            logText += "TIME\x02" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "\x01";
            logText += "ASSEMBLIES\x02";

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                logText += "  " + assembly.FullName + "\x02";
            }
            logText += "\x01";
            logText += "EXCEPTION-ASSEMBLY\x02" + Assembly.GetCallingAssembly().FullName + "\x01";
            logText += "EXCEPTION-THREAD\x02" + Thread.CurrentThread.Name + "\x01";

            int i = 0;
            while (exception != null)
            {
                logText += "EXCEPTION[" + i + "]-TYPE\x02" + exception.GetType().FullName + "\x01";
                logText += "EXCEPTION[" + i + "]-MESSAGE\x02" + exception.Message + "\x01";
                logText += "EXCEPTION[" + i + "]-STACKTRACE\x02" + exception.StackTrace + "\x01";

                i++;
                exception = exception.InnerException;
            }

            string path = Path.Combine(Environment.CurrentDirectory, "dumps",
                                       "stoperror-" + DateTime.UtcNow.Ticks + ".ihidump");

            File.WriteAllText(path, logText);
            Console.WriteLine();
            Console.WriteLine("Crash log saved to file:");
            Console.WriteLine("  " + path);
            Console.WriteLine("Press any key to exit (may take up to 5 seconds)");
            Console.ReadKey(true);
            
            if(CoreManager.ServerCore != null)
            {
                // Wait 5 seconds for everything to exit then force it.
                new Thread(() =>
                {
                    Thread.Sleep(5000);
                    Environment.Exit(100);
                })
                {
                    IsBackground = true,
                    Name = "BLUEDOT-StopErrorExiter"
                }.Start();

                CoreManager.ServerCore.Shutdown();
            }
        }

        private static void ShutdownKey(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
                return;

            e.Cancel = true;

            CoreManager.ServerCore.Shutdown(true);
        }
    }

    internal enum BootResult
    {
        AllClear,
        SocketBindingFailure,
        UnknownFailure
    }
}