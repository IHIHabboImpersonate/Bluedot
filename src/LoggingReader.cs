using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHI.Server
{
    public abstract class LoggingReader
    {
        public abstract void Debug(string channel, string text);
        public abstract void Notice(string channel, string text);
        public abstract void Important(string channel, string text);
        public abstract void Warning(string channel, string text);
        public abstract void Error(string channel, string text);
    }
}
