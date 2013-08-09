using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

using IHI.Server.Plugins;
using IHI.Server.Configuration;
using IHI.Server.Events;
using IHI.Server.Rooms;
using IHI.Server.Rooms.Figure;
using IHI.Server.Install;
using IHI.Server.Database;
using IHI.Server.Network.WebAdmin;
using IHI.Server.Permissions;
using IHI.Server.Habbos;
using IHI.Server.Network;

using Category = IHI.Server.Install.Category;
using System.Collections.Generic;
using IHI.Server.Network.GameSockets;
using System.ComponentModel;
using System.Threading;

namespace IHI.Server
{
    public class ServerCore
    {
        #region Fields
        #region Field: _bootInProgressLocker
        private readonly object _bootInProgressLocker = new object();
        #endregion
        
        #endregion

        #region Properties
        #region Property: MySqlConnectionProvider
        public MySqlConnectionProvider MySqlConnectionProvider
        {
            get;
            private set;
        }
        #endregion
        #region Property: Config
        public XmlConfig Config
        {
            get;
            private set;
        }
        #endregion
        #region Property: GameSocketManager
        public Dictionary<string, GameSocketManager> GameSocketManagers
        {
            get;
            private set;
        }
        #endregion
        #region Property: HabboDistributor
        public HabboDistributor HabboDistributor
        {
            get;
            private set;
        }
        #endregion
        #region Property: HabboFigureFactory
        public HabboFigureFactory HabboFigureFactory
        {
            get;
            private set;
        }
        #endregion
        #region Property: PermissionDistributor
        public PermissionDistributor PermissionDistributor
        {
            get;
            private set;
        }
        #endregion
        #region Property: StandardOutManager

        public log4net.ILog StandardOut
        {
            get;
            private set;
        }

        #region Property: StringLocale
        /// <summary>
        /// 
        /// </summary>
        public StringLocale StringLocale
        {
            get;
            private set;
        }
        #endregion

        public WebAdminManager WebAdminManager
        {
            get;
            private set;
        }
        #endregion
        #region Property: EventManager
        public EventManager EventManager
        {
            get;
            private set;
        }
        internal EventFirer OfficalEventFirer
        {
            get;
            private set;
        }
        #endregion
        #region Property: RoomDistributor
        public RoomDistributor RoomDistributor
        {
            get;
            private set;
        }
        #endregion
        #region Property: PluginManager
        public PluginManager PluginManager
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: ServerCore (Constructor)
        public ServerCore()
        {
            EventManager = new EventManager();
            OfficalEventFirer = new EventFirer(null);

            StringLocale = new StringLocale();
            PluginManager = new PluginManager();
            GameSocketManagers = new Dictionary<string, GameSocketManager>();
        }
        #endregion
        #region Method: Boot
        internal void Boot()
        {
            lock (_bootInProgressLocker)
            {
                StandardOut = log4net.LogManager.GetLogger("DEFAULT_STDOUT");
                StringLocale.SetDefaults();

                #region Ensure Directory Structure
                XmlConfig.EnsureDirectory(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dumps")));
                XmlConfig.EnsureDirectory(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins")));
                XmlConfig.EnsureDirectory(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libs")));
                #endregion
                
                BootTaskLoadConfig();
                BootTaskLoadPlugins();

                BootTaskRunInstaller();

                BootTaskConnectMySql();

                Task.WaitAll(new[]
                                 {
                                     Task.Factory.StartNew(BootTaskPreparePermissions),
                                     Task.Factory.StartNew(BootTaskPrepareFigures),
                                     Task.Factory.StartNew(BootTaskPrepareHabbos),
                                     Task.Factory.StartNew(BootTaskPrepareRooms)
                                 });

                Task.Factory.StartNew(BootTaskStartWebAdmin);

                BootTaskStartPlugins();

                StandardOut.Info("Core => " + StringLocale.GetString("CORE:BOOT_COMPLETE"));

                Console.Beep(500, 250);
            }
        }
        #endregion

        #region Method: BootTaskLoadConfig
        private void BootTaskLoadConfig()
        {
            string configPath = Environment.GetEnvironmentVariable("BLUEDOT_CONFIG_PATH");

            StandardOut.Info(StringLocale.GetString("CORE:BOOT_LOADING_CONFIG_AT") + configPath);
            Config = new XmlConfig(configPath);

            StandardOut.Info(StringLocale.GetString("CORE:BOOT_INSTALL_CHECKING"));
            bool mainInstallRequired = PrepareInstall(); // Register the main installation if required.
        }
        #endregion
        #region Method: BootTaskRunInstaller
        private void BootTaskRunInstaller()
        {
            if (CoreManager.InstallerCore.Run())
            {
                StandardOut.Info(StringLocale.GetString("CORE:BOOT_INSTALL_SAVING"));
                SaveConfigInstallation();
            }
        }
        #endregion

        #region Method: BootTaskConnectMySql
        private void BootTaskConnectMySql()
        {
            StandardOut.Info("MySQL => " + StringLocale.GetString("CORE:BOOT_MYSQL_PREPARE"));
            MySqlConnectionProvider = new MySqlConnectionProvider
            {
                Host = Config.ValueAsString("/config/mysql/host"),
                Port = Config.ValueAsUshort("/config/mysql/port", 3306),
                User = Config.ValueAsString("/config/mysql/user"),
                Password = Config.ValueAsString("/config/mysql/password"),
                Database = Config.ValueAsString("/config/mysql/database")
            };
            StandardOut.Info("MySQL => " + StringLocale.GetString("CORE:BOOT_MYSQL_READY"));
        }
        #endregion
        #region Method: BootTaskLoadPlugins
        private void BootTaskLoadPlugins()
        {
            List<Task> taskList = new List<Task>();
            StandardOut.Info("Plugin Manager => " + StringLocale.GetString("CORE:BOOT_PLUGINS_LOADING"));
            foreach (string path in PluginManager.GetAllPotentialPluginPaths())
            {
                taskList.Add(Task.Factory.StartNew(() => { PluginManager.LoadPluginAtPath(path); }));
            }
            Task.WaitAll(taskList.ToArray());
            StandardOut.Info("Plugin Manager => " + StringLocale.GetString("CORE:BOOT_PLUGINS_LOADED"));
        }
        #endregion
        #region Method: BootTaskStartPlugins
        private void BootTaskStartPlugins()
        {
            List<Task> taskList = new List<Task>();
            StandardOut.Info("Plugin Manager => " + StringLocale.GetString("CORE:BOOT_PLUGINS_STARTING"));
            foreach (Plugin plugin in PluginManager.GetLoadedPlugins())
            {
                taskList.Add(Task.Factory.StartNew(() => { PluginManager.StartPlugin(plugin); }));
            }
            Task.WaitAll(taskList.ToArray());
            StandardOut.Info("Plugin Manager => " + StringLocale.GetString("CORE:BOOT_PLUGINS_STARTED"));
        }
        #endregion


        #region Method: BootTaskPrepareFigures
        public void BootTaskPrepareFigures()
        {
            StandardOut.Info("Habbo Figure Factory => " + StringLocale.GetString("CORE:BOOT_FIGURES_PREPARE"));
            HabboFigureFactory = new HabboFigureFactory();
            StandardOut.Info("Habbo Figure Factory => " + StringLocale.GetString("CORE:BOOT_FIGURES_READY"));
        }
        #endregion
        #region Method: BootTaskPreparePermissions
        public void BootTaskPreparePermissions()
        {
            StandardOut.Info("Permission Distributor => " + StringLocale.GetString("CORE:BOOT_PERMISSIONS_PREPARE"));
            PermissionDistributor = new PermissionDistributor();
            StandardOut.Info("Permission Distributor => " + StringLocale.GetString("CORE:BOOT_PERMISSIONS_READY"));
        }
        #endregion
        #region Method: BootTaskPreparePermissions
        public void BootTaskPrepareHabbos()
        {
            StandardOut.Info("Habbo Distributor => " + StringLocale.GetString("CORE:BOOT_HABBODISTRIBUTOR_PREPARE"));
            HabboDistributor = new HabboDistributor();
            StandardOut.Info("Habbo Distributor => " + StringLocale.GetString("CORE:BOOT_HABBODISTRIBUTOR_READY"));
        }
        #endregion
        #region Method: BootTaskPreparePermissions
        public void BootTaskPrepareRooms()
        {
            StandardOut.Info("Room Distributor => " + StringLocale.GetString("CORE:BOOT_ROOMDISTRIBUTOR_PREPARE"));
            RoomDistributor = new RoomDistributor();
            StandardOut.Info("Room Distributor => " + StringLocale.GetString("CORE:BOOT_ROOMDISTRIBUTOR_READY"));
        }
        #endregion

        #region Method: NewGameSocketManager
        public GameSocketManager NewGameSocketManager(string protocolName, IPEndPoint ipEndpoint, GameSocketProtocol protocol)
        {
            return NewGameSocketManager(protocolName, ipEndpoint.Address, (ushort)ipEndpoint.Port, protocol);
        }
        public GameSocketManager NewGameSocketManager(string protocolName, ushort port, GameSocketProtocol protocol)
        {
            return NewGameSocketManager(protocolName, IPAddress.Any, port, protocol);
        }
        public GameSocketManager NewGameSocketManager(string socketManagerName, IPAddress ipAddress, ushort port, GameSocketProtocol protocol)
        {
            GameSocketManager gameSocketManager = new GameSocketManager
            {
                Address = IPAddress.Any,
                Port = port,
                Protocol = protocol
            };

            CancelEventArgs args = new CancelEventArgs();
            OfficalEventFirer.Fire("gamesocketmanager_added", EventPriority.Before, gameSocketManager, args);

            if (args.Cancel)
                return null;

            GameSocketManagers.Add(socketManagerName, gameSocketManager);
            OfficalEventFirer.Fire("gamesocketmanager_added", EventPriority.Before, gameSocketManager, args);
            return gameSocketManager;
        }
        #endregion

        #region Method: BootTaskStartWebAdmin
        public void BootTaskStartWebAdmin()
        {
            StandardOut.Info("Web Admin => " + StringLocale.GetString("CORE:BOOT_WEBADMIN_PREPARE"));
            WebAdminManager = new WebAdminManager(Config.ValueAsUshort("/config/webadmin/port", 14480));
            StandardOut.Info("Web Admin => " + StringLocale.GetString("CORE:BOOT_WEBADMIN_READY"));
        }
        #endregion

        #region Method: SubBootStepPrepareInstall
        private bool PrepareInstall()
        {
            if (Config.WasCreated) // Did the config file have to be created?
            {
                // Yes, add to the Installer Core.
                CoreManager.InstallerCore.
                    AddCategory(
                        "Database",
                        new Category().
                            AddStep("Host",
                                    new StringStep(
                                        "MySQL Host",
                                        "This is the Hostname or IP Address used to connect to the MySQL server.",
                                        new[]
                                            {
                                                "localhost",
                                                "127.0.0.1",
                                                "db.somedomain.com"
                                            },
                                        "localhost")).
                            AddStep("Port",
                                    new UShortStep(
                                        "MySQL Port",
                                        "This is the Port used to connect to the MySQL server.",
                                        new[]
                                            {
                                                "3306",
                                                "12345"
                                            },
                                        3306)).
                            AddStep("Username",
                                    new StringStep(
                                        "MySQL Username",
                                        "This is the Username used to authenticate with the MySQL server.",
                                        new[]
                                            {
                                                "ihi",
                                                "root (NOT RECOMMENDED)",
                                                "chris"
                                            },
                                        "ihi")).
                            AddStep("Password",
                                    new PasswordStep(
                                        "MySQL Password",
                                        "This is the Password used to authenticate with the MySQL server.")).
                            AddStep("DatebaseName",
                                    new StringStep(
                                        "MySQL Database Name",
                                        "This is the name of the database IHI.Server should use.",
                                        new[]
                                            {
                                                "ihi",
                                                "ihidb",
                                                "hotel"
                                            },
                                        "ihi")).
                            AddStep("MinimumPoolSize",
                                    new IntStep(
                                        "MySQL Minimum Pool Side",
                                        "This is the minimum amount of MySQL connections to maintain in the pool.",
                                        new[]
                                            {
                                                "1",
                                                "5"
                                            },
                                        1,
                                        1)).
                            AddStep("MaximumPoolSize",
                                    new IntStep(
                                        "MySQL Maximum Pool Side",
                                        "This is the maximum amount of MySQL connections to maintain in the pool.",
                                        new[]
                                            {
                                                "1",
                                                "5"
                                            },
                                        1,
                                        1))).
                    AddCategory("Network",
                                new Category().
                                    AddStep(
                                        "WebAdminPort",
                                        new UShortStep(
                                            "WebAdmin Port",
                                            "This is the port to bind the WebAdmin listener.",
                                            new[]
                                                {
                                                    "14480",
                                                    "30002"
                                                },
                                            14480)));
                return true;
            }
            return false;
        }
        #endregion
        #region Method: SubBootStepSaveConfigInstallation
        private void SaveConfigInstallation()
        {
            InstallerCore installer = CoreManager.InstallerCore;
            
            XmlDocument doc = Config.GetInternalDocument();
            XmlNode rootElement = doc.SelectSingleNode("/config");

            XmlElement standardOutElement = doc.CreateElement("standardout");
            XmlElement mySQLElement = doc.CreateElement("mysql");
            XmlElement networkElement = doc.CreateElement("network");
            XmlElement webAdminElement = doc.CreateElement("webadmin");

            #region StandardOutManager

            #region Importance

            XmlElement valueElement = doc.CreateElement("importance");
            valueElement.InnerText = installer.GetInstallerOutputValue("StandardOut", "Importance").ToString();
            standardOutElement.AppendChild(valueElement);

            #endregion

            #endregion

            #region MySQL

            #region Host

            valueElement = doc.CreateElement("host");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "Host").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region Port

            valueElement = doc.CreateElement("port");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "Port").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region User

            valueElement = doc.CreateElement("user");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "Username").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region Password

            valueElement = doc.CreateElement("password");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "Password").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region Database

            valueElement = doc.CreateElement("database");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "DatebaseName").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region MinPoolSize

            valueElement = doc.CreateElement("minpoolsize");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "MinimumPoolSize").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #region MaxPoolSize

            valueElement = doc.CreateElement("maxpoolsize");
            valueElement.InnerText = installer.GetInstallerOutputValue("Database", "MaximumPoolSize").ToString();
            mySQLElement.AppendChild(valueElement);

            #endregion

            #endregion

            #region Network

            #region Host

            valueElement = doc.CreateElement("host");
            valueElement.InnerText = installer.GetInstallerOutputValue("Network", "GameHost").ToString();
            networkElement.AppendChild(valueElement);

            #endregion

            #region Port

            valueElement = doc.CreateElement("port");
            valueElement.InnerText = installer.GetInstallerOutputValue("Network", "GamePort").ToString();
            networkElement.AppendChild(valueElement);

            #endregion

            #endregion

            #region WebAdmin

            #region Port

            valueElement = doc.CreateElement("port");
            valueElement.InnerText = installer.GetInstallerOutputValue("Network", "WebAdminPort").ToString();
            webAdminElement.AppendChild(valueElement);

            #endregion

            #endregion

            rootElement.AppendChild(standardOutElement);
            rootElement.AppendChild(mySQLElement);
            rootElement.AppendChild(networkElement);
            rootElement.AppendChild(webAdminElement);

            Config.Save();
        }
        #endregion
        
        #region Method: Shutdown
        public void Shutdown(bool directShutdown = false)
        {
            Console.TreatControlCAsInput = true;

            // If Config is not set then nothing is and we are already done.
            if (Config == null)
                return;

            lock (_bootInProgressLocker)
            {
                if (directShutdown)
                {
                    Console.Beep(1000, 100);
                    Console.Beep(1000, 100);
                    
                    // Clear out previous input.
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Are you sure you want to shutdown?");
                    Console.WriteLine("Press Y to confirm!");
                    Console.WriteLine();

                    if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    {
                        Console.TreatControlCAsInput = false;
                        Console.WriteLine("Shutdown cancelled.");
                        return;
                    }
                    Console.WriteLine("Shutting down. Please wait...");
                }

                Thread forceThread = new Thread(new ThreadStart(ForceShutdown));
                forceThread.IsBackground = true;
                forceThread.Start();

                CoreManager.ServerCore.OfficalEventFirer.Fire("shutdown", EventPriority.Before, this, EventArgs.Empty);
                CoreManager.ServerCore.OfficalEventFirer.Fire("shutdown", EventPriority.After, this, EventArgs.Empty);
                
                WebAdminManager.Stop();


                Console.Beep(4000, 100);
                Console.Beep(3500, 100);
                Console.Beep(3000, 100);
                Console.Beep(2500, 100);
                Console.Beep(2000, 100);
                Console.Beep(1500, 100);
                Console.Beep(1000, 100);
            }
        }

        private void ForceShutdown()
        {
            Thread.Sleep(2500);
            bool force = false;
            while (!force)
            {
                Thread.Sleep(5000);
                Console.Beep(400, 150);
                Console.Beep(700, 150);
                Console.Beep(400, 150);

                Console.WriteLine();
                Console.WriteLine();

                ConsoleColor oldColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Something is preventing the shutdown, would you like to force it?");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING: THIS MAY CAUSE DAMAGE TO YOUR DATABASE! DO SO AT YOUR OWN RISK!");
                Console.WriteLine("         This is however safer than killing the process externally (e.g. Task Manager)");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press F to FORCE close!");
                Console.ForegroundColor = oldColour;

                if (Console.ReadKey(true).Key != ConsoleKey.F)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Waiting 5 more seconds for shutdown...");
                    Console.ForegroundColor = oldColour;
                }
                else
                    force = true;
            }

            Console.Beep(400, 750);
            Console.Beep(400, 750);
            Console.Beep(400, 750);
            Environment.Exit(1);
        }

        #endregion
        #endregion
    }
}
