using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SessionCacher.Annotations;

namespace SessionCacher
{
    public class Session : INotifyPropertyChanged
    {
        public int id;
        public string name { get; set; }
        public List<Program> listOfPrograms;

        public List<Program> programs{

        get
        {
            return listOfPrograms;
        }

        set
            {
                if (value != listOfPrograms)
                {
                    this.listOfPrograms = value;
                    OnPropertyChanged();
                }
            } 
        }
        //TODO count and show number of proceses.

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

        //All below can be in controller
        public List<Program> ConvertProcesstoPrograms(List<Process> listOfProcesses)
        {
            var programList = new List<Program>();
            foreach (var process in listOfProcesses)
            {
                programList.Add(new Program(process.MainWindowTitle, "path", "",id));
            }
            return programList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
