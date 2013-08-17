using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHI.Server
{
    public class LoggingManager
    {
        public HashSet<LoggingReader> _readers;

        public LoggingManager()
        {
            _readers = new HashSet<LoggingReader>();
        }


        public LoggingManager Debug(string channel, string text)
        {
            foreach (LoggingReader reader in _readers)
            {
                reader.Debug(channel, text);
            }
            return this;
        }
        public LoggingManager Notice(string channel, string text)
        {
            foreach (LoggingReader reader in _readers)
            {
                reader.Notice(channel, text);
            }
            return this;
        }
        public LoggingManager Important(string channel, string text)
        {
            foreach (LoggingReader reader in _readers)
            {
                reader.Important(channel, text);
            }
            return this;
        }
        public LoggingManager Warning(string channel, string text)
        {
            foreach (LoggingReader reader in _readers)
            {
                reader.Warning(channel, text);
            }
            return this;
        }
        public LoggingManager Error(string channel, string text)
        {
            foreach (LoggingReader reader in _readers)
            {
                reader.Error(channel, text);
            }
            return this;
        }
    }
}
