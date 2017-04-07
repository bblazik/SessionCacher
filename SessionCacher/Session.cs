using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionCacher
{
    public class Session
    {
        private int id;
        private string name;
        public List<Program> listOfPrograms { get; set; }

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

        public Session(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


    }
}
