using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Habbos.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.Habbos
{
    public class Habbo : IMessageable, IPersistable, IBefriendable
    {
        #region Properties
        #region Property: Id
        /// <summary>
        /// The ID of this habbo.
        /// This value is read only.
        /// </summary>
        public int Id
        {
            get;
            private set;
        }
        #endregion
        #region Property: LoginId
        private int? _loginId;
        /// <summary>
        /// The login this Habbo is assigned to.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public int LoginId
        {
            get
            {
                if(!_loginId.HasValue)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _loginId = habboData.LoginId;
                    }
                }
                return _loginId.Value;
            }
            set
            {
                if (_loginId == value)
                    return;
                _loginId = value;
                
                // Update the database
                using(Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.LoginId = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion

        #region Property: Username
        private string _username;
        /// <summary>
        /// The username of this Habbo.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public string Username
        {
            get
            {
                if(_username == null)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _username = habboData.Username;
                    }
                }
                return _username;

            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Setting the username of a Habbo to null is not allowed!");
                if (_username == value)
                    return;

                _username = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.Username = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: DisplayName
        private string _displayName;
        /// <summary>
        /// The display name of this Habbo.
        /// If not set then this will return the username instead.
        /// This property is not saved in the database and is not maintained between instances.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName ?? _username;
            }
            set
            {
                _displayName = value;
            }
        }
        #endregion
        
        #region Property: DateCreated
        private DateTime? _dateCreated;
        /// <summary>
        /// The time and date this Habbo was created.
        /// This value is read only.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public DateTime DateCreated
        {
            get
            {
                if (!_dateCreated.HasValue)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _dateCreated = habboData.DateCreated;
                    }
                }
                return _dateCreated.Value;
            }
        }
        #endregion
        #region Property: LastAccess
        /// <summary>
        /// The time and date this Habbo last connected.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        private DateTime? _lastAccess;
        public DateTime LastAccess
        {
            get
            {
                if (!_lastAccess.HasValue)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _lastAccess = habboData.LastAccess;
                    }
                }
                return _lastAccess.Value;
            }
            set
            {
                if (_lastAccess == value)
                    return;
                _lastAccess = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.LastAccess = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: SSOTicket
        /// <summary>
        /// The SSO ticket of this Habbo.
        /// This property is not cached. It is always loaded/saved from/to the database.
        /// </summary>
        internal string SSOTicket
        {
            get
            {
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    return habboData.SSOTicket;
                }
            }
            set
            {
                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.SSOTicket = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: OriginIP
        /// <summary>
        /// The IP address this Habbo had the last time they logged in.
        /// This property is not cached. It is always loaded/saved from/to the database.
        /// DO NOT USE TO GET THE CURRENT IP!
        /// </summary>
        public IPAddress OriginIP
        {
            get
            {
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    return new IPAddress(habboData.RawOriginIP);
                }
            }
            set
            {
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.RawOriginIP = value.GetAddressBytes();
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: LoggedIn
        private bool _loggedIn;
        /// <summary>
        ///   Is the Habbo logged in.
        ///   Setting this to true will also update the LastAccess time to the current time.
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                lock (this)
                    return _loggedIn;
            }
            set
            {
                lock (this)
                {
                    if (!_loggedIn && value)
                    {
                        HabboEventArgs habboEventArgs = new HabboEventArgs();

                        CoreManager.ServerCore.EventManager.Fire("habbo_login:before", this, habboEventArgs);

                        if (habboEventArgs.Cancelled)
                        {
                            Socket.Disconnect();
                            return;
                        }

                        LastAccess = DateTime.Now;

                        CoreManager.ServerCore.EventManager.Fire("habbo_login:after", this, habboEventArgs);
                    }
                    _loggedIn = value;
                }
            }
        }
        #endregion

        #region Property: Figure
        private HabboFigure _figure;
        /// <summary>
        /// TODO: Document
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public HabboFigure Figure
        {
            get
            {
                if (_figure == null)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _figure = CoreManager.ServerCore.HabboFigureFactory.Parse(habboData.FigureString, habboData.FigureGender);
                    }
                }
                return _figure;
            }
        }
        #endregion
        #region Property: Motto
        private string _motto;
        /// <summary>
        /// The motto of this Habbo.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public string Motto
        {
            get
            {
                if (_motto == null)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _motto = habboData.Motto;
                    }
                }
                return _motto;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Setting the motto of a Habbo to null is not allowed!");
                if (_motto == value)
                    return;
                _motto = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.Motto = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: Credits
        private int? _credits;
        /// <summary>
        /// The amount of credits this Habbo has.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public int Credits
        {
            get
            {
                if (!_credits.HasValue)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _credits = habboData.Credits;
                    }
                }
                return _credits.Value;
            }
            set
            {
                if (_credits == value)
                    return;
                _credits = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.Credits = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: Pixels
        private int? _pixels;
        /// <summary>
        /// The amount of pixels this Habbo has.
        /// Changing this will automatically change it in the database too.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public int Pixels
        {
            get
            {
                if (!_pixels.HasValue)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _pixels = habboData.Pixels;
                    }
                }
                return _pixels.Value;
            }
            set
            {
                if (_pixels == value)
                    return;
                _pixels = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.Pixels = value;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion

        #region Property: MessengerCategories
        public ResettableLazy<EventingCollection<HashSet<MessengerCategory>, MessengerCategory>> MessengerCategories
        {
            get;
            private set;
        }
        #endregion
        #region IBefriendable Properties
        #region Property: Stalkable
        /// <summary>
        /// 
        /// </summary>
        public bool Stalkable
        {
            get;
            private set;
        }
        #endregion
        #region Property: Requestable
        /// <summary>
        /// 
        /// </summary>
        public bool Requestable
        {
            get;
            private set;
        }
        #endregion
        #region Property: Inviteable
        /// <summary>
        /// 
        /// </summary>
        public bool Inviteable
        {
            get;
            private set;
        }
        #endregion
        #endregion
        
        #region Property: Socket
        /// <summary>
        /// The current socket connection of this Habbo.
        /// </summary>
        public GameSocket Socket
        {
            get;
            private set;
        }
        #endregion

        #region Property: PersistentValues
        #region Property: PersistentValues
        private PersistentStorage _persistentValues;
        /// <summary>
        /// Document
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public PersistentStorage PersistentValues
        {
            get
            {
                if (_persistentValues == null)
                    _persistentValues = new PersistentStorage(this);
                return _persistentValues;
            }
        }
        #endregion
        #endregion
        #region Property: PersistInstance
        public long PersistInstanceProperty()
        {
            return Id;
        }
        #endregion

        #region Property: Permissions
        private IDictionary<Permission, PermissionState> _permissions;
        /// <summary>
        /// TODO: Document
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public IDictionary<Permission, PermissionState> Permissions
        {
            get
            {
                if (_permissions == null || _permissionGroups == null)
                {

                    List<DBHabboPermission> permissions;
                    List<DBHabboPermissionGroup> groups;
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        permissions = dbSession.HabboPermissions.Where(habbo => habbo.Id == Id).ToList();
                        groups = dbSession.HabboPermissionGroups.Where(habbo => habbo.Id == Id).ToList();
                    }

                    PermissionDistributor distributor = CoreManager.ServerCore.PermissionDistributor;

                    #region Permissions
                    _permissions = new Dictionary<Permission, PermissionState>();
                    foreach (DBHabboPermission permission in permissions)
                    {
                        // HACK: Native Enum support coming to a future Entity Framework version.
                        PermissionState permissionState;
                        if (!Enum.TryParse(permission.PermissionState, true, out permissionState))
                            continue;
                        _permissions.Add(distributor.GetPermission(permission.Id), permissionState);
                    }
                    #endregion
                    #region Groups
                    _permissionGroups = new HashSet<PermissionGroup>();
                    foreach (DBHabboPermissionGroup permissionGroup in groups)
                    {
                        _permissionGroups.Add(distributor.GetGroup(permissionGroup.Id));
                    }
                    #endregion
                }
                return _permissions;
            }
        }
        #endregion
        #region Property: PermissionGroups
        private ICollection<PermissionGroup> _permissionGroups;
        /// <summary>
        /// TODO: Document
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public ICollection<PermissionGroup> PermissionGroups
        {
            get
            {
                if (_permissions == null || _permissionGroups == null)
                {

                    List<DBHabboPermission> permissions;
                    List<DBHabboPermissionGroup> groups;
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        permissions = dbSession.HabboPermissions.Where(habbo => habbo.Id == Id).ToList();
                        groups = dbSession.HabboPermissionGroups.Where(habbo => habbo.Id == Id).ToList();
                    }

                    PermissionDistributor distributor = CoreManager.ServerCore.PermissionDistributor;

                    #region Permissions
                    _permissions = new Dictionary<Permission, PermissionState>();
                    foreach (DBHabboPermission permission in permissions)
                    {
                        // HACK: Native Enum support coming to a future Entity Framework version.
                        PermissionState permissionState;
                        if (!Enum.TryParse(permission.PermissionState, true, out permissionState))
                            continue;
                        _permissions.Add(distributor.GetPermission(permission.Id), permissionState);
                    }
                    #endregion
                    #region Groups
                    _permissionGroups = new HashSet<PermissionGroup>();
                    foreach (DBHabboPermissionGroup permissionGroup in groups)
                    {
                        _permissionGroups.Add(distributor.GetGroup(permissionGroup.Id));
                    }
                    #endregion
                }
                return _permissionGroups;
            }
        }
        #endregion

        #region Property: Badges
        private BadgeCollection _badges;
        /// <summary>
        /// TODO: Add summary.
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public BadgeCollection Badges
        {
            get
            {
                if (_badges == null)
                {
                    List<DBBadgeAssignment> badges;
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        badges = dbSession.BadgeAssignments.Where(habbo => habbo.HabboId == Id).ToList();
                    }
                    List<BadgeType> badgeAssignments = new List<BadgeType>();
                    BadgeTypeDistributor badgeTypeDistributor = CoreManager.ServerCore.BadgeTypeDistributor;
                    BadgeType[] slots = new BadgeType[5];
                    foreach (DBBadgeAssignment badge in badges)
                    {
                        badgeAssignments.Add(badgeTypeDistributor[badge.TypeId]);
                        switch (badge.Slot)
                        {
                            case "slot1":
                                slots[0] = badgeTypeDistributor[badge.TypeId];
                                break;
                            case "slot2":
                                slots[1] = badgeTypeDistributor[badge.TypeId];
                                break;
                            case "slot3":
                                slots[2] = badgeTypeDistributor[badge.TypeId];
                                break;
                            case "slot4":
                                slots[3] = badgeTypeDistributor[badge.TypeId];
                                break;
                            case "slot5":
                                slots[4] = badgeTypeDistributor[badge.TypeId];
                                break;
                            default:
                                break;
                        }
                    }
                    _badges = new BadgeCollection(badgeAssignments);
                    int i = 1;
                    foreach (BadgeType badge in slots)
                    {
                        if (badge != null)
                            _badges[(BadgeSlot)i] = badge;
                        i++;
                    }
                }
                return _badges;
            }
            set
            {
                // Setting to null causes the lazy loading to be reset.
                if (value == null)
                    _badges = null;
            }
        }
        #endregion
        
        #region Property: Subscriptions
        private SubscriptionCollection _subscriptions;
        /// <summary>
        /// 
        /// </summary>
        public ResettableLazy<SubscriptionCollection> Subscriptions
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: Habbo (Constructor)
        internal Habbo(int id)
        {
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                if (dbSession.Habbos.Any(habbo => habbo.Id == id))
                    Id = id;
            }
            InitLazy();
        }
        internal Habbo(string username)
        {
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Username == username);
                Id = habboData.Id;
                _username = habboData.Username;
            }
            InitLazy();
        }
        internal Habbo(GameSocket socket)
        {
            Socket = socket;
        }
        #endregion
        #region Method: InitLazy
        public void InitLazy()
        {
            MessengerCategories = new ResettableLazy<EventingCollection<HashSet<MessengerCategory>, MessengerCategory>>(() => new EventingCollection<HashSet<MessengerCategory>, MessengerCategory>());
            Subscriptions = new ResettableLazy<SubscriptionCollection>(() => new SubscriptionCollection(this));
        }
        #endregion
        #region Method: Login Merge
        public void LoginMerge(Habbo loggedInHabbo)
        {
            Socket = loggedInHabbo.Socket;
            Socket.Habbo = this;
        }
        #endregion
        #region Method: SendMessage
        public IMessageable SendMessage(IInternalOutgoingMessage message)
        {
#if DEBUG
            CoreManager.ServerCore.StandardOut.PrintDebugModeMessage("OUTGOING => " + message.Header + message.ContentString);
#endif
            Socket.Send(message.GetBytes());
            return this;
        }
        #endregion
        #region Method: GetPermissionState
        /// <summary>
        /// TODO: Document
        /// </summary>
        public PermissionState GetPermissionState(Permission permission)
        {
            #region Habbo
            if (_permissions.ContainsKey(permission))
                return _permissions[permission];
            #endregion
            #region Groups
            foreach (PermissionGroup permissionGroup in _permissionGroups)
            {
                PermissionState result = permissionGroup.GetPermissionState(permission);
                if (result != PermissionState.Undefined)
                    return result;
            }
            #endregion
            #region Defaults
            if (CoreManager.ServerCore.PermissionDistributor.DefaultPermissions.ContainsKey(permission))
                return CoreManager.ServerCore.PermissionDistributor.DefaultPermissions[permission];
            return PermissionState.Undefined;
            #endregion
        }
        #endregion
        #endregion
    }
}
