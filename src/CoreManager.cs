using IHI.Server.Install;

namespace IHI.Server
{
    public class CoreManager
    {
        /// <summary>
        ///   The instance of the server Core
        /// </summary>
        public static ServerCore ServerCore { get; private set; }

        /// <summary>
        ///   The instance of the installer Core.
        /// </summary>
        public static InstallerCore InstallerCore { get; private set; }

        internal static void InitialiseServerCore()
        {
            ServerCore = new ServerCore();
        }

        internal static void InitialiseInstallerCore()
        {
            InstallerCore = new InstallerCore();
        }
        internal static void DereferenceInstallerCore()
        {
            InstallerCore = null;
        }
    }
}
