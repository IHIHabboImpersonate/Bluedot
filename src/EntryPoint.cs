#region Usings

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace IHI.Server
{
    internal static class EntryPoint
    {
// ReSharper disable InconsistentNaming
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 MF_GRAYED = 0x00000001;
// ReSharper restore InconsistentNaming

        private static bool _waitOnBsod = false;

        internal static void Main(string[] arguments)
        {
            log4net.Config.BasicConfigurator.Configure();


            Console.Title = "Bluedot Habbo Server";

            log4net.ILog stdOut = log4net.LogManager.GetLogger("DEFAULT_STDOUT");
            stdOut.Info("Bluedot Habbo Server - Preparing...");

            #region Exit management
            stdOut.Debug("Disabling close window button...");
            // Disable close button to prevent unsafe closing.
            IntPtr current = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            EnableMenuItem(GetSystemMenu(current, false), SC_CLOSE, MF_GRAYED);

            stdOut.Debug("Binding shutdown to CTRL + C and CTRL + Break...");
            // Reassign CTRL+C and CTRL+BREAK to safely shutdown.
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += ShutdownKey;
            #endregion


            Thread.CurrentThread.Name = "BLUEDOT-EntryThread";

            stdOut.Debug("Setting up packaged reference loading...");
            // Allows embedded resources to be loaded.
            AppDomain.CurrentDomain.AssemblyResolve += LoadPackagedReferences;

            stdOut.Debug("Setting up fatel exception handler (BSOD style)...");
            // Bluescreen in the event of a fatal unhandled exception.
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;


            string configFile = Path.Combine(Environment.CurrentDirectory, "config.xml");

            stdOut.Debug("Parsing command line arguments...");
            Regex nameValueRegex = new Regex("^--(?<name>[\\w-]+)=(?<value>.+)$");

            foreach (string argument in arguments)
            {
                Match nameValueMatch = nameValueRegex.Match(argument);
                string name = nameValueMatch.Groups["name"].Value;
                string value = nameValueMatch.Groups["value"].Value;

                Console.WriteLine("  " + name + " - " + value);

                switch (name)
                {
                    case "config-file":
                        {
                            configFile = value;
                            break;
                        }
                    case "bsod-wait":
                        {
                            _waitOnBsod = true;
                            break;
                        }
                }
            }
            stdOut.Info("Config location: " + configFile);
            Environment.SetEnvironmentVariable("BLUEDOT_CONFIG_PATH", configFile);

            stdOut.Info("Preparing installer core...");
            CoreManager.InitialiseInstallerCore();

            stdOut.Info("Preparing server core...");
            CoreManager.InitialiseServerCore();

            stdOut.Info("Starting server core...");
            CoreManager.ServerCore.Boot();
        }

        private static Assembly LoadPackagedReferences(object sender, ResolveEventArgs args)
        {
            return LoadPackagedDll(new AssemblyName(args.Name).Name);
        }
        public static Assembly LoadPackagedDll(string name)
        {
            String resourceName = "IHI.Server.Reference_Packaging." + name + ".dll";

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
            
            if(_waitOnBsod)
            {
                log4net.ILog stdOut = log4net.LogManager.GetLogger("DEFAULT_STDOUT");
                stdOut.Fatal("BLUEDOT STOP ERROR - Press any key to show!");
                Console.ReadKey(true);
            }

            #region Bluescreen Output
            Console.WindowWidth = Console.BufferWidth = Console.WindowWidth * 2;

            Exception exception = e.ExceptionObject as Exception;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine("[===[ BLUEDOT STOP ERROR ]===]");
            Console.WriteLine("An unhandled exception has caused Bluedot to close!");
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
            #endregion

            string path = Path.Combine(Environment.CurrentDirectory, "dumps",
                                       "stoperror-" + DateTime.UtcNow.Ticks + ".ihidump");

            File.WriteAllText(path, logText);
            Console.WriteLine();
            Console.WriteLine("Crash log saved to file:");
            Console.WriteLine("  " + path);
            Console.WriteLine("Press any key to exit (may take up to 5 seconds)");
            Console.ReadKey(true);
            
            // If the server is at least partially started then give it a chance to shut down safely.
            if(CoreManager.ServerCore != null)
            {
                // Wait at least 5 seconds for everything to exit then force it.
                new Thread(() =>
                {
                    Thread.Sleep(5000);
                    Environment.Exit(1);
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
            // If the server hasn't started, just exit.
            if (CoreManager.ServerCore == null)
                Environment.Exit(0);

            // We can't stop CTRL+BREAK closing so we should just force a safe shutdown.
            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
                CoreManager.ServerCore.Shutdown();
            else
            {
                // We CAN stop CTRL+C closing so we should confirm the shutdown first.
                e.Cancel = true;
                CoreManager.ServerCore.Shutdown(true);
            }
        }
    }
}