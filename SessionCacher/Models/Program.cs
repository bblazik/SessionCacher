using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionCacher
{
    public class Program
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        private int SessionID { get; set; }

        public Program(string name, string path, string arguments)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
        }

        public Program(string id, string name, string path, string arguments, int sessionId)
        {
            this.id = id;
            Name = name;
            Path = path;
            Arguments = arguments;
            SessionID = sessionId;
        }

        public Program(string name, string path, string arguments, int sessionId)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
            SessionID = sessionId;
        }

        public string GetValues()
        {
            return string.Join("','", Name, Path, Arguments, SessionID);
        }
    }
}
