#region Usings

using IHI.Server.Database.Actions;
using IHI.Server.Network;
using IHI.Server.Useful;

#endregion

namespace IHI.Server.Habbos
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
            _idCache = new WeakCache<int, Habbo>(id => new Habbo(id));
            _usernameCache = new WeakCache<string, Habbo>(username => new Habbo(username));
        }
        #endregion

        #region Method: GetHabboFromSSOTicket
        /// <summary>
        ///   Returns the Habbo with the matching SSO Ticket.
        ///   If no match is made, null is returned.
        /// </summary>
        /// <param name = "ssoTicket">The SSO Ticket to match.</param>
        public Habbo GetHabboFromSSOTicket(string ssoTicket)
        {
            // Get the ID of the matching Habbo.
            int habboId = HabboActions.GetHabboIdFromSSOTicket(ssoTicket);

            // Is the ID is -1
            if (habboId == -1)
            {
                // Yes, meaning there was no match. Return null.
                return null;
            }

            // No, return the Habbo from the cache.
            return this[habboId];
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
        #endregion
    }
}