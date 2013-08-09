#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace IHI.Server.Plugins
{
    public class PluginManager
    {
        private readonly Dictionary<string, Plugin> _plugins = new Dictionary<string, Plugin>();

        /// <summary>
        /// Removed (To be rewritten) - Reason: Wrong documention is worse than none.
        /// </summary>
        /// <param name = "name">Removed (To be rewritten) - Reason: Wrong documention is worse than none.</param>
        internal Plugin GetPlugin(string path)
        {
            if (_plugins.ContainsKey(path))
                return _plugins[path];
            return null;
        }

        /// <summary>
        ///   Start a plugin.
        /// </summary>
        /// <param name = "plugin">The plugin object you wish to start.</param>
        internal PluginManager StartPlugin(Plugin plugin)
        {
            EventFirer eventFirer = new EventFirer(plugin);
            plugin.Start(eventFirer);
            plugin.StartedResetEvent.Set();
            CoreManager.ServerCore.StandardOut.Info(CoreManager.ServerCore.StringLocale.GetString("CORE:PLUGIN_STARTED", plugin.Name));
            return this;
        }

        /// <summary>
        ///   Load a plugin at a given path.
        /// </summary>
        /// <param name = "path">The file path of the plugin.</param>
        internal Plugin LoadPluginAtPath(string path)
        {
            if (_plugins.ContainsKey(path))
                return _plugins[path];

            if (!new FileInfo(path).Exists)
            {
                CoreManager.ServerCore.StandardOut.Warn(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_PLUGIN_NOT_EXIST", path));
                return null;
            }

            Assembly pluginAssembly = Assembly.LoadFile(path);
            Type genericPluginType = typeof(Plugin);
            Type specificPluginType = pluginAssembly.GetTypes().FirstOrDefault(T => T.IsSubclassOf(genericPluginType));

            if (specificPluginType == null)
            {
                CoreManager.ServerCore.StandardOut.Warn(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_PLUGIN_NOT_EXIST", Path.GetFileNameWithoutExtension(path)));
                CoreManager.ServerCore.StandardOut.Debug(path);
                return null;
            }

            Plugin pluginInstance = Activator.CreateInstance(specificPluginType) as Plugin;

            if (pluginInstance.Name == null)
                pluginInstance.Name = Path.GetFileNameWithoutExtension(path);

            _plugins.Add(path, pluginInstance);

            return pluginInstance;
        }

        /// <summary>
        ///   Returns a string array containing the paths of all DLL files in the plugins directory.
        /// </summary>
        internal static IEnumerable<string> GetAllPotentialPluginPaths()
        {
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), "*.dll",
                                      SearchOption.AllDirectories);
        }

        /// <summary>
        ///   Returns a Plugin array containing all the loaded plugins.
        /// </summary>
        public IEnumerable<Plugin> GetLoadedPlugins()
        {
            return _plugins.Values;
        }
    }
}