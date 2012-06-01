using System.Collections.Generic;

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
        public PermissionGroup()
        {
            Permissions = new Dictionary<Permission, PermissionState>();
            ChildGroups = new HashSet<PermissionGroup>();
        }
        #endregion
        #region Method: GetPermissionState
        /// <summary>
        /// TODO: This could do with a good bit of documentation. The behavour is not clear.
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
