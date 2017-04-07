using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionCacher
{
    public class Session
    {
        public int id;
        public string name;
        public List<Program> listOfPrograms { get; set; }


        // Constructors
        public Session()
        {
        }

        public Session(string name)
        {
            this.name = name;
        }

        public Session(string name, List<Program> listOfPrograms)
        {
            this.name = name;
            this.listOfPrograms = listOfPrograms;
        }

        public Session(string name, List<Process> listOProcesses)
        {
            this.name = name;
            this.listOfPrograms = ConvertProcesstoPrograms(listOProcesses);
        }

        public Session(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        // End of constructors.

        public List<Program> ConvertProcesstoPrograms(List<Process> listOfProcesses)
        {
            var programList = new List<Program>();
            foreach (var process in listOfProcesses)
            {
                programList.Add(new Program(process.MainWindowTitle, "path", "",id));
            }
            return programList;
        }
    }
}
