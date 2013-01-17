using System.Collections.Generic;

using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class MFuseRights : OutgoingMessage
    {
        public ICollection<string> FuseRights
        {
            get;
            set;
        }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(2);
                
                InternalOutgoingMessage.AppendInt32(FuseRights.Count);
                foreach (string fuseRight in FuseRights)
                {
                    InternalOutgoingMessage.AppendString(fuseRight);
                }
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}