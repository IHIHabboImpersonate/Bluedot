using System;
using System.IO;
using System.Net;
using System.Xml;

using Bluedot.HabboServer.ApiUsage;
using Bluedot.HabboServer.ApiUsage.Plugins;
using Bluedot.HabboServer.Configuration;
using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Network.StandardOut;
using Bluedot.HabboServer.Rooms;
using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Install;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Network.WebAdmin;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

using Category = Bluedot.HabboServer.Install.Category;

namespace Bluedot.HabboServer
{

    internal class ServerCore
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
        public GameSocketManager GameSocketManager
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
        #region Property: FuseRightManager
        public FuseRightManager FuseRightManager
        {
            get;
            private set;
        }
        #endregion
        #region Property: StandardOutManager
        public StandardOutManager StandardOutManager
        {
            get;
            private set;
        }
        public WebAdminManager WebAdminManager
        {
            get;
            private set;
        }
        #endregion
        #region Property: EventManager
        /// <summary>
        /// TODO: Add summary for property
        /// </summary>
        public EventManager EventManager
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
        #endregion

        #region Methods
        #region Method: ServerCore (Constructor)
        public ServerCore()
        {
            EventManager = new EventManager();
        }
        #endregion
        #region Method: Boot
        internal void Boot(string configPath)
        {
            lock (_bootInProgressLocker)
            {
                #region Ensure Directory Structure
                //XmlConfig.EnsureDirectory(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dumps")));
                #endregion
                
                #region Config & Installation
                Console.WriteLine("Loading config file at: " + configPath);
                Config = new XmlConfig(configPath);

                Console.WriteLine("Checking if basic installation tasks are required...");
                bool mainInstallRequired = PreInstall(); // Register the main installation if required.
                CoreManager.InstallerCore.Run();
                if (mainInstallRequired)
                {
                    Console.WriteLine("Saving config file...");
                    SaveConfigInstallation();
                }
                #endregion
                
                #region Standard Out
                ushort standardOutPort = Config.ValueAsUshort("/config/network/standardoutport", 14481);
                Console.WriteLine("Starting StandardOutManager on port " + standardOutPort);
                StandardOutManager = new StandardOutManager(standardOutPort);
                Console.WriteLine("StandardOutManager started!");

                Console.WriteLine("The rest of the start up output will also be sent to the debug channel.");
                #endregion

                #region MySQL
                BasicConsoleWrite("MySQL => Creating connection provider");
                MySqlConnectionProvider = new MySqlConnectionProvider
                                              {
                                                  Host = Config.ValueAsString("/config/mysql/host"),
                                                  Port = Config.ValueAsUshort("/config/mysql/port", 3306),
                                                  User = Config.ValueAsString("/config/mysql/user"),
                                                  Password = Config.ValueAsString("/config/mysql/password"),
                                                  Database = Config.ValueAsString("/config/mysql/database")
                                              };
                BasicConsoleWrite("MySQL => Connection provider ready!");
                #endregion

                #region Figure Factory
                BasicConsoleWrite("Habbo Figure Factory => Constructing...");
                HabboFigureFactory = new HabboFigureFactory();
                BasicConsoleWrite("Habbo Figure Factory => Ready");
                #endregion

                #region Distributors
                #region PermissionDistributor
                BasicConsoleWrite("Permission Distributor => Constructing...");
                PermissionDistributor = new PermissionDistributor();
                BasicConsoleWrite("Permission Distributor => Ready");
                #endregion

                #region FuseRightManager
                BasicConsoleWrite("Fuse Right Manager => Constructing...");
                FuseRightManager = new FuseRightManager();
                BasicConsoleWrite("Fuse Right Manager => Ready");
                #endregion

                #region HabboDistributor
                BasicConsoleWrite("Habbo Distributor => Constructing...");
                HabboDistributor = new HabboDistributor();
                BasicConsoleWrite("Habbo Distributor => Ready");
                #endregion

                #region RoomDistributor
                BasicConsoleWrite("Room Distributor => Constructing...");
                RoomDistributor = new RoomDistributor();
                BasicConsoleWrite("Room Distributor => Ready");
                #endregion
                #endregion

                #region Network
                #region Game Socket
                BasicConsoleWrite("Game Socket Manager => Starting...");
                GameSocketManager = new GameSocketManager
                                        {
                                            Address = IPAddress.Any,
                                            Port = Config.ValueAsUshort("/config/network/port", 14478),
                                            Reader = new ClassicGameSocketReader()
                                        };
                GameSocketManager.Start();
                BasicConsoleWrite("Game Socket Manager => Ready!");
                #endregion

                #region WebAdmin
                BasicConsoleWrite("WebAdmin => Starting...");
                WebAdminManager = new WebAdminManager(Config.ValueAsUshort("/config/webadmin/port", 14480));
                BasicConsoleWrite("WebAdmin => Ready!");
                #endregion

                #endregion

                BasicConsoleWrite("Plugin NavigatorManager => Starting Pseudo Plugin System...");
                ApiCallerRoot.Start();
                Console.Beep(500, 250);
                BasicConsoleWrite("Core => Bluedot Habbo Server is now functional!");
                _disableConventialStandardOut = true;

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Output is no longer sent to conventional StandardOut!");
                Console.WriteLine("Connect to the StardardOutManager on port " + standardOutPort);
            }
        }
        #endregion
        #region Method: PreInstall
        private bool PreInstall()
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
                                        "This is the name of the database IHI should use.",
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
                                        "GameHost",
                                        new StringStep(
                                            "Game Host",
                                            "This is the host (normally an IP) to bind the listener for normal game connections.",
                                            new[]
                                                {
                                                    "127.0.0.1",
                                                    "192.168.1.12",
                                                    "5.24.246.133"
                                                },
                                            "127.0.0.1")).
                                    AddStep(
                                        "GamePort",
                                        new UShortStep(
                                            "Game Port",
                                            "This is the port to bind the listener for normal game connections.",
                                            new[]
                                                {
                                                    "14478",
                                                    "30000"
                                                },
                                            14478)).
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
        #region Method: SaveConfigInstallation
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
        public void Shutdown(bool confirm = false)
        {
            Console.TreatControlCAsInput = true;

            // If Config is not set then nothing is and we are already done.
            if (Config == null)
                return;

            lock (_bootInProgressLocker)
            {
                if (confirm)
                {
                    Console.Beep(1000, 100);
                    Console.Beep(1000, 100);
                    
                    // Clear out previous input.
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Are you sure you want to shutdown Bluedot?");
                    Console.WriteLine("Press Y to confirm! ");
                    Console.WriteLine();

                    if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    {
                        Console.TreatControlCAsInput = false;
                        Console.WriteLine("Shutdown cancelled.");
                        return;
                    }
                    Console.WriteLine("Shutting down. Please wait...");
                }

                EventManager.Fire("shutdown", this, EventArgs.Empty);
                
                GameSocketManager.Stop();
                WebAdminManager.Stop();

                StandardOutManager.Stop();

                Console.Beep(4000, 100);
                Console.Beep(3500, 100);
                Console.Beep(3000, 100);
                Console.Beep(2500, 100);
                Console.Beep(2000, 100);
                Console.Beep(1500, 100);
                Console.Beep(1000, 100);
            }
        }

        #endregion

        #region Method: BasicConsoleWrite
        private bool _disableConventialStandardOut = false;
        internal void BasicConsoleWrite(string message)
        {
            if (!_disableConventialStandardOut)
                Console.WriteLine(message);
            
            try
            {
                StandardOutManager.DebugChannel.WriteMessage(message);
            }
            catch {}
        }
        #endregion
        #endregion
    }
}
