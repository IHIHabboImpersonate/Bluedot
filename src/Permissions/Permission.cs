namespace Bluedot.HabboServer.Permissions
{
    public class Permission 
    {
        #region Properties
        #region Property: Name
        /// <summary>
        /// The name of the permission.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: Permission (Constructor)
        internal Permission(string name)
        {
            Name = name;
        }
        #endregion
        #endregion
    }
}
