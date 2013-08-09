using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHI.Server.Permissions
{
    public class FuseRightEventArgs : EventArgs
    {
        private HashSet<string> _fuseRights;

        public FuseRightEventArgs()
        {
            _fuseRights = new HashSet<string>();
        }

        public ICollection<string> GetFuseRights()
        {
            return _fuseRights;
        }

        public FuseRightEventArgs AddFuseRight(string fuseRight)
        {
            _fuseRights.Add(fuseRight);
            return this;
        }
    }
}
