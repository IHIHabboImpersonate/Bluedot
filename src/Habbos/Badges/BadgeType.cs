using System.Linq;
using Bluedot.HabboServer.Database;

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
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                Code =  dbSession
                            .BadgeTypes
                                .Where(badgeType => badgeType.Id == id)
                                .Select(row => new { row.Code })
                                .Single().Code;
            }
        }
        internal BadgeType(string code)
        {
            Code = code;
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                Id =  dbSession
                        .BadgeTypes
                            .Where(badgeType => badgeType.Code == code)
                            .Select(row => new {row.Id})
                            .Single().Id;
            }
        }
        #endregion
    }
}
