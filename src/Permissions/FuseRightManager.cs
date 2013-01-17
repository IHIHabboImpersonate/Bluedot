#region Usings

using System;
using System.Collections.Generic;
using System.Threading;

using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Habbos;


#endregion


namespace Bluedot.HabboServer.Permissions
{
    /// <summary
    /// Handles fuse right management. Fuse rights should only be used when required by the client.
    /// All permissions should be handled using the permission system.
    /// </summary>
    public class FuseRightManager
    {
        #region Fields
        #region Field: _permissionGroupCache
        private readonly IDictionary<string, Func<string, IDictionary<string, PermissionState>, bool>> _fuseRightResolvers;
        #endregion
        #region Field: _resolverDictionLock
        /// <summary>
        /// 
        /// </summary>
        private readonly ReaderWriterLockSlim _resolverDictionLock;
        #endregion
        #endregion

        #region Methods
        #region Method: PermissionDistributor (Constructor)
        public FuseRightManager()
        {
            _resolverDictionLock = new ReaderWriterLockSlim();
            _fuseRightResolvers = new Dictionary<string, Func<string, IDictionary<string, PermissionState>, bool>>();
        }
        #endregion

        #region Method: RegisterFuseRight
        public bool RegisterFuseRight(string fuseRight, Func<string, IDictionary<string, PermissionState>, bool> resolver = null)
        {
            _resolverDictionLock.EnterUpgradeableReadLock();
            try
            {
                if (!_fuseRightResolvers.ContainsKey(fuseRight))
                {
                    _resolverDictionLock.EnterWriteLock();
                    try
                    {
                        if (resolver == null)
                            resolver = DefaultFuseResolver;
                        _fuseRightResolvers.Add(fuseRight, resolver);
                        return true;
                    }
                    finally
                    {
                        _resolverDictionLock.ExitWriteLock();
                    }
                }
                return false;
            }
            finally
            {
                _resolverDictionLock.ExitUpgradeableReadLock();
            }
        }
        #endregion

        #region Method: UnregisterFuseRight
        public bool UnregisterFuseRight(string fuseRight)
        {
            _resolverDictionLock.EnterUpgradeableReadLock();
            try
            {
                if (_fuseRightResolvers.ContainsKey(fuseRight))
                {
                    _resolverDictionLock.EnterWriteLock();
                    try
                    {
                        _fuseRightResolvers.Remove(fuseRight);
                    }
                    finally
                    {
                        _resolverDictionLock.ExitWriteLock();
                    }
                }
                return false;
            }
            finally
            {
                _resolverDictionLock.ExitUpgradeableReadLock();
            }
        }
        #endregion

        #region Method: ResolveFuserights
        public IEnumerable<string> ResolvePermissions(IDictionary<string, PermissionState> permissions)
        {
            _resolverDictionLock.EnterReadLock();
            try
            {
                foreach (KeyValuePair<string, Func<string, IDictionary<string, PermissionState>, bool>> fuseRightResolver in _fuseRightResolvers)
                {
                    if (fuseRightResolver.Value(fuseRightResolver.Key, permissions))
                        yield return fuseRightResolver.Key;
                }
            }
            finally
            {
                _resolverDictionLock.ExitReadLock();
            }
        }
        #endregion

        #region Method: DefaultFuseResolver
        public bool DefaultFuseResolver(string fuseRight, IDictionary<string, PermissionState> permissions)
        {
            return CoreManager.ServerCore.PermissionDistributor.HasPermission(permissions, "fuse:" + fuseRight);
        }
        #endregion
        #endregion
    }
}