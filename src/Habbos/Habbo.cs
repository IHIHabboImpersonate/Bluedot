using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Habbos.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using SmartWeakEvent;

namespace Bluedot.HabboServer.Habbos
{
    public class Habbo : IMessageable, IPersistable
    {
        #region Events
        #region Event: OnPreLogin
        private readonly FastSmartWeakEvent<HabboEventHandler> _eventOnPreLogin = new FastSmartWeakEvent<HabboEventHandler>();
        /// <summary>
        /// Invoked when the LoggedIn property is attempted to be changed to true.
        /// Cancelling this event will cause the connected client to be disconnected 
        /// and the value of LoggedIn will remain false.
        /// </summary>
        public event HabboEventHandler OnPreLogin
        {
            add { _eventOnPreLogin.Add(value); }
            remove { _eventOnPreLogin.Remove(value); }
        }
        #endregion
        #region Event: OnAnyPreLogin
        private static readonly FastSmartWeakEvent<HabboEventHandler> _eventOnAnyPreLogin = new FastSmartWeakEvent<HabboEventHandler>();
        /// <summary>
        /// Invoked when the LoggedIn property is attempted to be changed to true for any Habbo.
        /// Cancelling this event will cause the connected client to be disconnected 
        /// and the value of LoggedIn will remain false.
        /// </summary>
        public static event HabboEventHandler OnAnyPreLogin
        {
            add { _eventOnAnyPreLogin.Add(value); }
            remove { _eventOnAnyPreLogin.Remove(value); }
        }
        #endregion

        #region Event: OnLogin
        private readonly FastSmartWeakEvent<HabboEventHandler> _eventOnLogin = new FastSmartWeakEvent<HabboEventHandler>();
        /// <summary>
        /// Invoked when the LoggedIn property is changed to true.
        /// Cancelling this event has no affect.
        /// </summary>
        public event HabboEventHandler OnLogin
        {
            add { _eventOnLogin.Add(value); }
            remove { _eventOnLogin.Remove(value); }
        }
        #endregion
        #region Event: OnAnyLogin
        private static readonly FastSmartWeakEvent<HabboEventHandler> _eventOnAnyLogin = new FastSmartWeakEvent<HabboEventHandler>();
        /// <summary>
        /// Invoked when the LoggedIn property is changed to true for any Habbo.
        /// Cancelling this event has no affect.
        /// </summary>
        public static event HabboEventHandler OnAnyLogin
        {
            add { _eventOnAnyLogin.Add(value); }
            remove { _eventOnAnyLogin.Remove(value); }
        }
        #endregion

        #region Event: OnMessageSent
        private readonly FastSmartWeakEvent<GameSocketMessageEvent> _eventOnMessageSent = new FastSmartWeakEvent<GameSocketMessageEvent>();
        /// <summary>
        /// Invoked when a ever a message is sent to this IMessageable.
        /// </summary>
        public event GameSocketMessageEvent OnMessageSent
        {
            add { _eventOnMessageSent.Add(value); }
            remove { _eventOnMessageSent.Remove(value); }
        }
        #endregion
        #region Event: OnAnyMessageSent
        private static readonly FastSmartWeakEvent<GameSocketMessageEvent> _eventOnAnyMessageSent = new FastSmartWeakEvent<GameSocketMessageEvent>();
        /// <summary>
        /// Invoked when a ever a message is sent to any Habbo instance.
        /// </summary>
        public static event GameSocketMessageEvent OnAnyMessageSent
        {
            add { _eventOnAnyMessageSent.Add(value); }
            remove { _eventOnAnyMessageSent.Remove(value); }
        }
        #endregion
        #endregion

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
        /// <summary>
        /// The time and date this Habbo was created.
        /// This value is read only.
        /// </summary>
        public DateTime DateCreated
        {
            // TODO: Add lazy loading
            get;
            private set;
        }
        #endregion
        #region Property: LastAccess
        /// <summary>
        /// The time and date this Habbo last connected.
        /// Changing this will automatically change it in the database too.
        /// </summary>
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
                        _eventOnPreLogin.Raise(this, habboEventArgs);
                        _eventOnAnyPreLogin.Raise(this, habboEventArgs);

                        if (habboEventArgs.Cancelled)
                        {
                            Socket.Disconnect();
                            return;
                        }

                        LastAccess = DateTime.Now;

                        _eventOnLogin.Raise(this, habboEventArgs);
                        _eventOnAnyLogin.Raise(this, habboEventArgs);
                    }
                    _loggedIn = value;
                }
            }
        }
        #endregion

        #region Property: Figure
        public HabboFigure _figure;
        public HabboFigure Figure
        {
            get
            {
                if (_figure == null)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                        _motto = habboData.Motto;
                    }
                }
                return _figure;
            }
            set
            {
                if (_figure == value)
                    return;
                _figure = value;

                // Update the database
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);
                    habboData.FigureGender = _figure.Gender;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion
        #region Property: SwimFigure
        // TODO: SwimFigure stuff
        #endregion
        #region Property: Motto
        private string _motto;
        /// <summary>
        /// The motto of this Habbo.
        /// Changing this will automatically change it in the database too.
        /// </summary>
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
        #region Property: Messenger
        private HabboMessenger _messenger;
        internal HabboMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = new HabboMessenger(this);
                return _messenger;
            }
        }
        #endregion

        #region Property: PersistentValues
        public PersistentStorage PersistentValues
        {
            get;
            private set;
        }
        #endregion
        #region Property: PersistInstance
        public long PersistInstanceProperty()
        {
            return Id;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: Habbo (Constructor)
        internal Habbo(int id)
        {
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                if(dbSession.Habbos.Any(habbo => habbo.Id == id))
                    Id = id;
            }
            PersistentValues = new PersistentStorage(this);
        }
        internal Habbo(string username)
        {
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Username == username);

                Id = habboData.Id;
                _username = habboData.Username;
            }
            PersistentValues = new PersistentStorage(this);
        }
        internal Habbo(GameSocket socket)
        {
            Socket = socket;
            PersistentValues = new PersistentStorage(this);
        }
        #endregion
        #region Method: Login Merge
        public void LoginMerge(Habbo loggedInHabbo)
        {
            Socket = loggedInHabbo.Socket;
            Socket.Habbo = this;
        }
        #endregion
        #region Method: RefreshFromDatabase
        /// <summary>
        /// Sets all property values to the value stored in the database.
        /// </summary>
        public void RefreshFromDatabase(bool privateData = true, bool friendData = true, bool internalData = true)
        {
            // TODO: Lazy loading
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                DBHabbo habboData = dbSession.Habbos.Single(habbo => habbo.Id == Id);

                if (privateData)
                {
                    _loginId = habboData.LoginId;
                    _pixels = habboData.Pixels;
                    _credits = habboData.Credits;
                    DateCreated = habboData.DateCreated;
                }
                if (privateData || friendData || internalData)
                {
                    _username = habboData.Username;
                    _motto = habboData.Motto;
                    _lastAccess = habboData.LastAccess;
                }
            }
        }
        #endregion
        #region Method: SendMessage
        public IMessageable SendMessage(IInternalOutgoingMessage message)
        {
            Socket.Send(message.GetBytes());

            _eventOnMessageSent.Raise(this, new GameSocketMessageEventArgs(message));
            _eventOnAnyMessageSent.Raise(this, new GameSocketMessageEventArgs(message));
            return this;
        }
        #endregion
        #endregion
    }
}
