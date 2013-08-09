#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Bluedot.HabboServer.Network
{
    public abstract class OutgoingMessage
    {
        protected readonly IInternalOutgoingMessage InternalOutgoingMessage = new InternalOutgoingMessage();

        public abstract OutgoingMessage Send(IMessageable target);

        public OutgoingMessage Send(IEnumerable<IMessageable> targets, bool sendOncePerConnection = false)
        {
            if (sendOncePerConnection)
                throw new NotImplementedException("sendOncePerConnection not implemented");

            foreach (IMessageable target in targets)
            {
                Send(target);
            }
            return this;
        }
    }
}