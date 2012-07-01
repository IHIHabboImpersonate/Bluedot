
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

using System.Linq;
using System.Net;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

#endregion

namespace Bluedot.HabboServer.Habbos
{
    public class HabboDistributor
    {
        #region Fields
        #region Field: _idCache
        private readonly WeakCache<int, Habbo> _idCache;
        #endregion
        #region Field: _usernameCache
        private readonly WeakCache<string, Habbo> _usernameCache;
        #endregion
        #endregion

        #region Indexers
        #region Indexer: int
        public Habbo this[int id]
        {
            get
            {
                Habbo result = _idCache[id];
                _usernameCache[result.Username] = result;
                return result;
            }
        }
        #endregion
        #region Indexer: string
        public Habbo this[string username]
        {
            get
            {
                Habbo result = _usernameCache[username];
                _idCache[result.Id] = result;
                return result;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: HabboDistributor (Constructor)
        public HabboDistributor()
        {
            _idCache = new WeakCache<int, Habbo>(CacheInstanceGenerator);
            _usernameCache = new WeakCache<string, Habbo>(CacheInstanceGenerator);
        }
        #endregion

        #region Method: GetHabbo
        /// <summary>
        ///   Returns a Habbo with a matching SSO Ticket and Origin IP.
        ///   If no match is made, null is returned.
        /// </summary>
        /// <param name = "ssoTicket">The SSO Ticket to match.</param>
        /// <param name = "origin">The IP Address to match.</param>
        public Habbo GetHabbo(string ssoTicket, IPAddress origin)
        {
            byte[] addressBytes = origin.GetAddressBytes();

            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                var habboData = dbSession.
                                    Habbos.
                                        Where(
                                            habbo => habbo.SSOTicket == ssoTicket && 
                                            habbo.RawOriginIP == addressBytes).
                                            Select(habbo => new { habbo.Id }).SingleOrDefault();

                if (habboData == null)
                    return null;
                return this[habboData.Id];
            }
        }
        #endregion
        #region Method: GetPreLoginHabbo
        /// <summary>
        ///   Creates a minimal Habbo object.
        ///   This is not cached and is only used after the Habbo connects but before logging in.
        ///   Do not use this Habbo for custom features. Use a cached version.
        /// </summary>
        /// <param name = "socket">The Connection this Habbo is for.</param>
        /// <returns>A mostly non-function Habbo.</returns>
        public static Habbo GetPreLoginHabbo(GameSocket socket)
        {
            return new Habbo(socket);
        }
        #endregion
        #region Method: CleanUp
        /// <summary>
        ///   Remove any collected Habbos from the cache.
        /// </summary>
        private void CleanUp()
        {
            // TODO: Look into calling this with http://msdn.microsoft.com/en-us/library/system.gc.registerforfullgcnotification.aspx
            _idCache.CleanUp();
            _usernameCache.CleanUp();
        }
        #endregion

        #region Method: CacheInstanceGenerator
        public Habbo CacheInstanceGenerator(int id)
        {
            return new Habbo(id);
        }
        public Habbo CacheInstanceGenerator(string username)
        {
            return new Habbo(username);
        }
        #endregion
        #endregion
    }
}