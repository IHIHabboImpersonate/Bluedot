using System;
using System.Linq;
using System.Collections.Generic;
using Bluedot.HabboServer.Database;

namespace Bluedot.HabboServer.Permissions
{
    public class PermissionGroup
    {
        #region Properties
        #region Property: Permissions
        /// <summary>
        /// The permissions this group specifies directly.
        /// </summary>
        public Dictionary<Permission, PermissionState> Permissions
        {
            get;
            private set;
        }
        #endregion
        #region Property: ChildGroups
        /// <summary>
        /// 
        /// </summary>
        public HashSet<PermissionGroup> ChildGroups
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: PermissionGroup (Constructor)
        public PermissionGroup(int id)
        {
            List<DBPermissionGroupPermission> permissions;
            List<DBPermissionGroupGroup> groups;
            using(Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                permissions = dbSession.PermissionGroupPermissions.Where(groupPermission => groupPermission.GroupId == id).ToList();
                groups = dbSession.PermissionGroupGroups.Where(groupPermission => groupPermission.GroupId == id).ToList();
            }

            PermissionDistributor distributor = CoreManager.ServerCore.PermissionDistributor;

            Permissions = new Dictionary<Permission, PermissionState>();
            foreach (DBPermissionGroupPermission groupPermission in permissions)
            {
                // HACK: Native Enum support coming to a future Entity Framework version.
                PermissionState permissionState;
                if(!Enum.TryParse(groupPermission.PermissionState, true, out permissionState))
                    continue;
                Permissions.Add(distributor.GetPermission(groupPermission.PermissionId), permissionState);
            }

            ChildGroups = new HashSet<PermissionGroup>();
            foreach (DBPermissionGroupGroup childGroup in groups)
            {
                ChildGroups.Add(distributor.GetGroup(childGroup.GroupId));
            }
        }
        #endregion
        #region Method: GetPermissionState
        /// <summary>
        /// TODO: Document
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public PermissionState GetPermissionState(Permission permission)
        {
            if (Permissions.ContainsKey(permission))
                return Permissions[permission];

            PermissionState result = PermissionState.Undefined;
            foreach (PermissionGroup childGroup in ChildGroups)
            {
                result = childGroup.GetPermissionState(permission);
                if (result != PermissionState.Undefined)
                    break;
            }
            return result;
        }
        #endregion
        #endregion
    }
}