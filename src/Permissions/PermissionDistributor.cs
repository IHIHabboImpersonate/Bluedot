#region Usings

using System.Collections.Generic;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Habbos;


#endregion


namespace Bluedot.HabboServer.Permissions
{

    public class PermissionDistributor
    {
        #region Constants
        #region Constant: DefaultPermissionsHabboId
        private const int DefaultPermissionsHabboId = -1; // TODO: Make this a config option.
        #endregion
        #endregion

        #region Fields
        #region Field: _permissionGroupCache
        private readonly IDictionary<string, IDictionary<string, PermissionState>> _permissionGroupCache;
        #endregion
        #region Field: _defaultPermissions
        private readonly IDictionary<string, PermissionState> _defaultPermissions;
        #endregion
        #endregion

        #region Properties
        #region Property: DefaultPermissions
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, PermissionState> DefaultPermissions
        {
            get
            {
                return _defaultPermissions;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: PermissionDistributor (Constructor)
        public PermissionDistributor()
        {
            _permissionGroupCache = new Dictionary<string, IDictionary<string, PermissionState>>();

            foreach (PermissionsGroupPermissionData permission in PermissionActions.GetAllPermissionGroupPermissions())
            {
                if (!_permissionGroupCache.ContainsKey(permission.GroupName))
                    _permissionGroupCache.Add(new KeyValuePair<string, IDictionary<string, PermissionState>>());
                _permissionGroupCache[permission.GroupName].Add(permission.PermissionName, permission.PermissionState);
            }
            _defaultPermissions = new Dictionary<string, PermissionState>();

            CoreManager.ServerCore.StandardOut.Info("Permissions => " + CoreManager.ServerCore.StringLocale.GetString("CORE:BOOT_PERMISSIONS_CALCULATE"));
            _defaultPermissions = GetHabboPermissions(DefaultPermissionsHabboId);
        }
        #endregion

        #region Method: GetHabboPermission
        public IDictionary<string, PermissionState> GetHabboPermissions(Habbo habbo)
        {
            return GetHabboPermissions(habbo.Id);
        }
        private IDictionary<string, PermissionState> GetHabboPermissions(int habboId)
        {
            IDictionary<string, PermissionState> permissions = new Dictionary<string, PermissionState>();

            using (WrappedMySqlConnection connection = CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                foreach (KeyValuePair<string, PermissionState> permission in PermissionActions.GetHabboPermissionsFromHabboId(habboId, connection))
                {
                    permissions.Add(permission);
                }
                foreach (string groupName in PermissionActions.GetHabboPermissionGroupsFromHabboId(habboId, connection))
                {
                    if (!_permissionGroupCache.ContainsKey(groupName))
                    {
                        CoreManager.ServerCore.StandardOut.Warn("Permissions => " + CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_PERMISSIONS_UNDEFINED_GROUP", groupName, habboId));
                        continue;
                    }
                    foreach (KeyValuePair<string, PermissionState> permission in _permissionGroupCache[groupName])
                    {
                        if (permissions.ContainsKey(permission.Key)) continue; // Individual permissions get priority over PermissionGroup permissions.
                        permissions.Add(permission);
                    }
                }
            }

            return permissions;
        }
        #endregion
        #endregion

        internal bool HasPermission(IDictionary<string, PermissionState> permissions, string permission)
        {
            if (!permissions.ContainsKey(permission))
                return false;
            return permissions[permission] == PermissionState.Allow;
        }
    }
}