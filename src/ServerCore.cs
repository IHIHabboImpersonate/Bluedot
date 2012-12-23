using System;
using System.IO;
using System.Net;
using System.Xml;
using Bluedot.HabboServer.ApiUsage;
using Bluedot.HabboServer.Configuration;
using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Habbos.Figure;
using Bluedot.HabboServer.Install;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Network.WebAdmin;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

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
        #region Property: StandardOut
        public StandardOut StandardOut
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
        #region Property: BadgeTypeDistributor
        /// <summary>
        /// TODO: Add summary for property
        /// </summary>
        public BadgeTypeDistributor BadgeTypeDistributor
        {
            get;
            private set;
        }
        #endregion
        #region Property: BadgeTypeDistributor
        /// <summary>
        /// TODO: Add summary for property
        /// </summary>
        public MySqlConnectionProvider MySqlConnectionProvider
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

                XmlConfig.EnsureDirectory(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dumps")));

                #endregion

                #region Standard Out

                StandardOut = new StandardOut();
                StandardOut.PrintNotice("Standard Out", "Ready");

                #endregion

                #region Config & Installation

                Config = new XmlConfig(configPath);

                bool mainInstallRequired = PreInstall(); // Register the main installation if required.
                CoreManager.InstallerCore.Run();
                if (mainInstallRequired)
                    SaveConfigInstallation();

                StandardOut.PrintNotice("Config", "Main config file loaded");
                StandardOut.SetImportance(
                    (StandardOutImportance)
                    Config.ValueAsByte("/config/standardout/importance", (byte) StandardOutImportance.Debug));

                #endregion

                #region MySQL

                StandardOut.PrintNotice("MySQL", "Creating connection provider");
                MySqlConnectionProvider = new MySqlConnectionProvider
                                              {
                                                  Host = Config.ValueAsString("/config/mysql/host"),
                                                  Port = Config.ValueAsUshort("/config/mysql/port", 3306),
                                                  User = Config.ValueAsString("/config/mysql/user"),
                                                  Password = Config.ValueAsString("/config/mysql/password"),
                                                  Database = Config.ValueAsString("/config/mysql/database")
                                              };
                StandardOut.PrintNotice("MySQL", "Connection provider ready!");

                #endregion

                #region Figure Factory

                StandardOut.PrintNotice("Habbo Figure Factory", "Constructing...");
                HabboFigureFactory = new HabboFigureFactory();
                StandardOut.PrintNotice("Habbo Figure Factory", "Ready");

                #endregion

                #region Distributors

                #region PermissionDistributor

                StandardOut.PrintNotice("Permission Distributor", "Constructing...");
                PermissionDistributor = new PermissionDistributor();
                StandardOut.PrintNotice("Permission Distributor", "Ready");

                #endregion

                #region HabboDistributor

                StandardOut.PrintNotice("Habbo Distributor", "Constructing...");
                HabboDistributor = new HabboDistributor();
                StandardOut.PrintNotice("Habbo Distributor", "Ready");

                #endregion

                #region BadgeTypeDistributor

                StandardOut.PrintNotice("Badge Type Cache", "Constructing...");
                BadgeTypeDistributor = new BadgeTypeDistributor();
                StandardOut.PrintNotice("Badge Type Cache", "Ready");

                #endregion

                #endregion

                #region Network

                #region Game Socket

                StandardOut.PrintNotice("Game Socket Manager", "Starting...");
                GameSocketManager = new GameSocketManager
                                        {
                                            Address = IPAddress.Any,
                                            Port = 14478,
                                            Reader = new ClassicGameSocketReader()
                                        };
                GameSocketManager.Start();
                StandardOut.PrintNotice("Game Socket Manager", "Ready!");

                #endregion

                #region WebAdmin

                StandardOut.PrintNotice("WebAdmin", "Starting...");
                WebAdminManager = new WebAdminManager(Config.ValueAsUshort("/config/webadmin/port", 14480));
                StandardOut.PrintNotice("WebAdmin", "Ready!");

                #endregion

                #endregion
                
                StandardOut.PrintImportant("Plugin Manager", "Starting Pseudo Plugin System...");
                ApiCallerRoot.Start();
                Console.Beep(500, 250);
                StandardOut.PrintImportant("Core", "Bluedot Habbo Server is now functional!");
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
                        "StandardOut",
                        new Category().
                            AddStep("Importance",
                                    new StringStep(
                                        "Default Importance",
                                        "This is the minimum importance level that messages must have to be printed to standard out.",
                                        new[]
                                            {
                                                "DEBUG",
                                                "NOTICE",
                                                "IMPORTANT",
                                                "WARNING",
                                                "ERROR"
                                            },
                                        "NOTICE"))).
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


            StandardOut.PrintImportant("Config", "Updating configuration file... (Install)");

            XmlDocument doc = Config.GetInternalDocument();
            XmlNode rootElement = doc.SelectSingleNode("/config");

            XmlElement standardOutElement = doc.CreateElement("standardout");
            XmlElement mySQLElement = doc.CreateElement("mysql");
            XmlElement networkElement = doc.CreateElement("network");
            XmlElement webAdminElement = doc.CreateElement("webadmin");

            #region StandardOut

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
            StandardOut.PrintImportant("Config", "Configuration file saved!");
        }
        #endregion
        #region Method: Shutdown
        public void Shutdown(bool confirm = false)
        {
            Console.TreatControlCAsInput = true;

            // If StandardOut is not started then nothing is and we are already done.
            if (StandardOut == null)
                return;

            lock (_bootInProgressLocker)
            {
                if (confirm)
                {
                    Console.Beep(1000, 100);
                    Console.Beep(1000, 100);

                    StandardOut.Hidden = true;

                    Console.Clear();
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Are you sure you want to shutdown Bluedot?");
                    Console.Write("Press Y to confirm! ");

                    if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    {
                        StandardOut.Hidden = false;
                        Console.TreatControlCAsInput = false;
                        return;
                    }
                    StandardOut.Hidden = false;
                }

                EventManager.Fire("shutdown", this, EventArgs.Empty);
                
                GameSocketManager.Stop();
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

        #endregion
        #endregion
    }
}
