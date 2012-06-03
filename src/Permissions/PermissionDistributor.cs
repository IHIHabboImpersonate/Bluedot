#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General internal License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General internal License for more details.
// 
// You should have received a copy of the GNU General internal License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using Bluedot.HabboServer.Cache;
using Bluedot.HabboServer.Database;

#endregion


namespace Bluedot.HabboServer.Permissions
{
    public class PermissionDistributor
    {
        #region Fields
        #region Field: _permissionCache
        private readonly WeakCache<string, Permission> _permissionCache;
        #endregion
        #region Field: _permissionGroupCache
        private readonly WeakCache<int, PermissionGroup> _permissionGroupCache;
        #endregion
        #region Field: _permissionIdNameLookup
        private readonly Dictionary<int, string> _permissionIdNameLookup;
        #endregion
        #endregion

        #region Property: DefaultPermissions
        private readonly IDictionary<Permission, PermissionState> _defaultPermissions;
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<Permission, PermissionState> DefaultPermissions
        {
            get
            {
                return _defaultPermissions;
            }
        }
        #endregion
        #region Property: DefaultPermissionGroups
        private readonly ICollection<PermissionGroup> _defaultPermissionGroups;
        /// <summary>
        /// 
        /// </summary>
        public ICollection<PermissionGroup> DefaultPermissionGroups
        {
            get
            {
                return _defaultPermissionGroups;
            }
        }
        #endregion

        #region Method: PermissionDistributor (Constructor)
        public PermissionDistributor()
        {
            _permissionCache = new WeakCache<string, Permission>(ConstructPermission);
            _permissionGroupCache = new WeakCache<int, PermissionGroup>(ConstructPermissionGroup);
            
            List<DBPermission> permissions;
            List<DBHabboPermission> defaultPermissions;
            List<DBHabboPermissionGroup> defaultGroups;
            using(Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                permissions = dbSession.Permissions.ToList();
                defaultPermissions = dbSession.HabboPermissions.Where(permission => permission.HabboId == null).ToList();
                defaultGroups = dbSession.HabboPermissionGroups.Where(group => group.HabboId == null).ToList();
            }
            #region ID to Name Lookup
            _permissionIdNameLookup = new Dictionary<int, string>(permissions.Count);
            foreach (DBPermission permission in permissions)
                _permissionIdNameLookup.Add(permission.Id, permission.Name);
            #endregion
            #region Default Permissions

            _defaultPermissions = new Dictionary<Permission, PermissionState>();
            foreach (DBHabboPermission defaultPermission in defaultPermissions)
            {
                // HACK: Native Enum support coming to a future Entity Framework version.
                PermissionState permissionState;
                if (!Enum.TryParse(defaultPermission.PermissionState, true, out permissionState))
                    continue;
                _defaultPermissions.Add(GetPermission(defaultPermission.Id), permissionState);
            }
            #endregion
            #region Default Groups
            _defaultPermissionGroups = new HashSet<PermissionGroup>();
            foreach (DBHabboPermissionGroup defaultGroup in defaultGroups)
            {
                _defaultPermissionGroups.Add(GetGroup(defaultGroup.Id));
            }
            #endregion
        }
        #endregion

        #region Method: ConstructPermission
        internal Permission ConstructPermission(string name)
        {
            return new Permission(name);
        }
        #endregion
        #region Method: ConstructPermissionGroup
        internal PermissionGroup ConstructPermissionGroup(int id)
        {
            return new PermissionGroup(id);
        }
        #endregion

        #region Method: RegisterPermission
        /// <summary>
        /// TODO: Document
        /// </summary>
        public PermissionDistributor RegisterPermission()
        {
            // TODO: Save to database.
            return this;
        }
        #endregion
        #region Method: DeregisterPermission
        /// <summary>
        /// TODO: Document
        /// </summary>
        public PermissionDistributor DeregisterPermission()
        {
            // TODO: Remove from database.
            return this;
        }
        #endregion

        #region Method: GetGroup
        public PermissionGroup GetGroup(int id)
        {
            return _permissionGroupCache[id];
        }
        #endregion
        #region Method: GetPermission
        public Permission GetPermission(int id)
        {
            if (!_permissionIdNameLookup.ContainsKey(id))
                return null;
            return _permissionCache[_permissionIdNameLookup[id]];
        }
        public Permission GetPermission(string name)
        {
            return _permissionCache[name];
        }
        #endregion
    }
}