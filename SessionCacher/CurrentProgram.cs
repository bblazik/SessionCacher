using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class CurrentProgram
    {
        public ObservableCollection<string> list;
        public CurrentProgram()
        {
            list = new ObservableCollection<string>();
            list.CollectionChanged += listChanged;
        }

        private void listChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                   list.Add( p.MainModule.FileName) ;
                    //p.MainModule.FileName;
                }
            }
        }
    }

