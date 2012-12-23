using Bluedot.HabboServer.Database.Actions;

namespace Bluedot.HabboServer.Habbos
{
    public class BadgeType
    {
        #region Property: Id
        /// <summary>
        /// TODO: Add Badge Id summary
        /// </summary>
        public int Id
        {
            get;
            private set;
        }
        #endregion
        #region Property: Code
        /// <summary>
        /// TODO: Add Badge Code summary
        /// </summary>
        public string Code
        {
            get;
            private set;
        }
        #endregion

        #region Method: BadgeType (Constructor)
        internal BadgeType(int id)
        {
            Id = id;
            Code = BadgeActions.GetBadgeTypeCodeFromBadgeTypeId(id);
        }
        internal BadgeType(string code)
        {
            Id = BadgeActions.GetBadgeTypeIdFromBadgeTypeCode(code);
            Code = code;
        }
        #endregion
    }
}
