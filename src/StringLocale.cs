using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHI.Server
{
    public class StringLocale
    {
        private const string FALLBACK_PREFIX = "NO_LOCALE_FOUND";

        private Dictionary<string, string> _strings;
        private HashSet<string> _readOnlyKeys;

        #region Method: StringLocale (Constructor)
        public StringLocale()
        {
            _strings = new Dictionary<string, string>();
            _readOnlyKeys = new HashSet<string>();
        }
        #endregion

        public string GetString(string key)
        {
            string returnString;

            if (!_strings.TryGetValue(key, out returnString))
                return FALLBACK_PREFIX + "[\"" + key + "\"]";

            return returnString;
        }

        public string GetString(string key, object arg0)
        {
            return String.Format(GetString(key), arg0);
        }
        public string GetString(string key, object arg0, object arg1)
        {
            return String.Format(GetString(key), arg0, arg1);
        }
        public string GetString(string key, object arg0, object arg1, object arg2)
        {
            return String.Format(GetString(key), arg0, arg1, arg2);
        }
        public string GetString(string key, params object[] args)
        {
            return String.Format(GetString(key), args);
        }
        public StringLocale SetString(string key, string value, bool readOnly = false)
        {
            if (_readOnlyKeys.Contains(key))
                throw new ReadOnlyException(GetString("CORE:ERROR_CONFIG_CREATION_FAILED", key));

            if (readOnly)
                _readOnlyKeys.Add(key);

            _strings[key] = value;

            return this;
        }

        #region Method: SetDefaults
        internal void SetDefaults()
        {
            SetString("CORE:PROJECT_NAME",                          "IHI Server", true);
            SetString("CORE:BOOT_COMPLETE",                         "IHI Server is now functional!");
            SetString("CORE:BOOT_LOADING_CONFIG_AT",                "Loading config file at: ");
            SetString("CORE:BOOT_INSTALL_CHECKING",                 "Checking if basic installation tasks are required...");
            SetString("CORE:BOOT_INSTALL_SAVING",                   "Saving config file...");
            SetString("CORE:BOOT_MYSQL_PREPARE",                    "Creating connection provider");
            SetString("CORE:BOOT_MYSQL_READY",                      "Connection provider ready!");
            SetString("CORE:BOOT_FIGURES_PREPARE",                  "Constructing...");
            SetString("CORE:BOOT_FIGURES_READY",                    "Ready");
            SetString("CORE:BOOT_PERMISSIONS_PREPARE",              "Constructing...");
            SetString("CORE:BOOT_PERMISSIONS_CALCULATE",            "Calculating default permissions for all Habbos.");
            SetString("CORE:BOOT_PERMISSIONS_READY",                "Ready");
            SetString("CORE:BOOT_HABBODISTRIBUTOR_PREPARE",         "Constructing...");
            SetString("CORE:BOOT_HABBODISTRIBUTOR_READY",           "Ready");
            SetString("CORE:BOOT_ROOMDISTRIBUTOR_PREPARE",          "Constructing...");
            SetString("CORE:BOOT_ROOMDISTRIBUTOR_READY",            "Ready");
            SetString("CORE:BOOT_WEBADMIN_PREPARE",                 "Starting...");
            SetString("CORE:BOOT_WEBADMIN_READY",                   "Ready");
            SetString("CORE:EXCEPTION_LOCALE_READONLY",             "The locale string key \"{0}\"  is marked as read only and cannot be changed.");
            SetString("CORE:EXCEPTION_HABBO_SET_MOTTO_NULL",        "Setting the motto of a Habbo to null is not allowed!");
            SetString("CORE:EXCEPTION_HABBO_CONSTRUCT_BAD_ID",      "Habbo ID doesn't exist!"); // TODO: Improve this exception to actually be helpful.
            SetString("CORE:ERROR_CONFIG_CREATION_FAILED",          "Config file \"{0}\"does not exist and couldn't be created automatically! (XmlConfig)");
            SetString("CORE:ERROR_CONFIG_INVALID_BYTE",             "Error: '{0}' is not a valid byte! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_SBYTE",            "Error: '{0}' is not a valid ubyte! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_SHORT",            "Error: '{0}' is not a valid short! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_USHORT",           "Error: '{0}' is not a valid ushort! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_INT",              "Error: '{0}' is not a valid int! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_UINT",             "Error: '{0}' is not a valid uint! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_LONG",             "Error: '{0}' is not a valid long! Fallback: {1}");
            SetString("CORE:ERROR_CONFIG_INVALID_ULONG",            "Error: '{0}' is not a valid ulong! Fallback: {1}");
            SetString("CORE:ERROR_NETWORK_INVALID_DATA",            "Invalid packet data. Are you sure you are using the correct GameSocketReader?");
            SetString("CORE:INFO_NETWORK_CONNECTION_CLOSED",        "Client Connection Closed: {0}");
            SetString("CORE:ERROR_NETWORK_CONNECTION_KILLED",       "Client Connection Killed: ");
            SetString("CORE:DEBUG_WEBADMIN_HANDLER_ADDED",          "Handler added: {0}");
            SetString("CORE:DEBUG_WEBADMIN_HANDLER_CHANGED",        "Handler changed: {0}");
            SetString("CORE:DEBUG_WEBADMIN_HANDLER_REMOVED",        "Handler removed: {0}");
            SetString("CORE:ERROR_WEBADMIN_OS_NOT_SUPPORTED",       "The WebAdmin is not supported on this operating system.");
            SetString("CORE:ERROR_WEBADMIN_STOP_FAILED",            "Unable to stop the WebAdmin process.");
            SetString("CORE:ERROR_WEBADMIN_INIT_FAILED",            "The WebAdmin handling process was not properly initialized so it could not be started.");
            SetString("CORE:ERROR_WEBADMIN_START_FAILED",           "Unable to start the request handling process.");
            SetString("CORE:ERROR_WEBADMIN_PORT_CONFLICT",          "The WebAdminServer was unable to start. Is the port already in use?");
            SetString("CORE:ERROR_PERMISSIONS_UNDEFINED_GROUP",     "Undefined PermissionGroup \"{0}\" referenced by Habbo ID: {1}");
            SetString("CORE:ERROR_ROOM_MODEL_MISSING",              "Failed to load a room with model \"{0}\" because the model type was not found");
            SetString("CORE:ERROR_PLUGIN_NOT_EXIST",                "Plugin does not exist: {0}");
            SetString("CORE:ERROR_PLUGIN_NOT_PLUGIN",               "{0} is in the plugin directory but is not a plugin.");
            SetString("CORE:PLUGIN_STARTED",                        "Plugin {0} has been started.");
            SetString("CORE:BOOT_PLUGINS_LOADING",                  "Loading plugins...");
            SetString("CORE:BOOT_PLUGINS_LOADED",                   "Plugins loaded!");
            SetString("CORE:BOOT_PLUGINS_STARTING",                 "Starting plugins...");
            SetString("CORE:BOOT_PLUGINS_STARTED",                  "Plugins started!");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
            SetString("CORE:",                                      "");
        }
        #endregion
    }
}