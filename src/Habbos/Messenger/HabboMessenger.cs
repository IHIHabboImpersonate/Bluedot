using System.Collections.Generic;
using System.Linq;
using Bluedot.HabboServer.Database;

namespace Bluedot.HabboServer.Habbos.Messenger
{
    internal class HabboMessenger
    {
        internal HabboMessenger(Habbo owner)
        {
            Owner = owner;
        }


        internal Habbo Owner
        {
            get;
            private set;
        }


        internal IEnumerable<Habbo> RecievedFriendRequests
        {
            get
            {
                HabboDistributor habboDistributor = CoreManager.ServerCore.HabboDistributor;

                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    foreach (DBMessengerFriendRequest request in dbSession.MessengerFriendRequests.Where(request => request.ToId == Owner.Id))
                    {
                        yield return habboDistributor[request.FromId];
                    }
                }
            }
        }
        internal IEnumerable<Habbo> SentFriendRequests
        {
            get
            {
                HabboDistributor habboDistributor = CoreManager.ServerCore.HabboDistributor;

                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    foreach (DBMessengerFriendRequest request in dbSession.MessengerFriendRequests.Where(request => request.FromId == Owner.Id))
                    {
                        yield return habboDistributor[request.ToId];
                    }
                }
            }
        }
        internal IEnumerable<Habbo> Friendships
        {
            get
            {
                HabboDistributor habboDistributor = CoreManager.ServerCore.HabboDistributor;

                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    foreach (DBMessengerFriendship friendship in dbSession.MessengerFriendships.Where(friendship => friendship.HabboAId == Owner.Id))
                    {
                        yield return habboDistributor[friendship.HabboBId];
                    }
                    foreach (DBMessengerFriendship friendship in dbSession.MessengerFriendships.Where(friendship => friendship.HabboBId == Owner.Id))
                    {
                        yield return habboDistributor[friendship.HabboAId];
                    }
                }
            }
        }
    }
}
