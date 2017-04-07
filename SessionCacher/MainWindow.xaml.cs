﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Data.SQLite;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SessionCacher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Process> processes;
        private DBHandler dbHandler;
        private List<Session> sessions;

        public MainWindow()
        {
            InitializeComponent();
            var c = new CurrentProgram();
            dbHandler = new DBHandler();
            //TODO initilize ALL LIST
            sessions = new List<Session>();
            GetCurrentSession();

            sessions = getSessionsFromDB();
            addToSessionsTab(sessions);
            
            //TODO ADD REVERT ACTION.            
        }

        public List<Session> getSessionsFromDB()
        {
            return dbHandler.GetSessionsWithProgramList();
        }

        public void addToSessionsTab(List<Session> sessions)
        {
            SavedSessions.Items.Clear();
            foreach (var session in sessions)
            {
                SavedSessions.Items.Add(session.Name);
            }
        }

        public void addToProgramsTab(Session session)
        {
            OpenedPrograms.Items.Clear();
            foreach (var program in session.listOfPrograms)
            {
                OpenedPrograms.Items.Add(program);
            }
        }


        public void GetCurrentSession()
        {
            processes = Process.GetProcesses().ToList();
            processes.RemoveAll(x => string.IsNullOrEmpty(x.MainWindowTitle));
            foreach (var p in processes)
            {
                if (!string.IsNullOrEmpty(p.MainWindowTitle))
                {
                    OpenedPrograms.Items.Add(p.MainModule.FileName);
                }
            }
            sessions.Add(new Session("Current session", ConvertProcesstoPrograms(processes)));
        }

        //TODO move to Session. Add argument id.
        public List<Program> ConvertProcesstoPrograms(List<Process> listOfProcesses)
        {
            var programList = new List<Program>();
            foreach (var process in listOfProcesses)
            {
                programList.Add(new Program(process.MainWindowTitle, "path", ""));
            }
            return programList;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenedPrograms.Items.Clear();
            GetCurrentSession();
        }

        private void save_Click_1(object sender, RoutedEventArgs e)
        {
            //Create session
            //TODO dialog box, czy dodac do biezącej sesji etc.
            var id = dbHandler.InsertSessionToTable(new Session("CokolwiekNarazie"));
            //add to session
            foreach (var process in processes)
            {
                dbHandler.InsertProgramToTable(new Program(process.MainWindowTitle,"path", "",id));
                //TODO process.MainModule.FileName get privilages.
                //TODO Get Arguments
                //TODO success dialog.    
            }

            sessions = getSessionsFromDB();
            addToSessionsTab(sessions);
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (SavedSessions.SelectedIndex != -1)
            {
                var index = SavedSessions.SelectedIndex;
                var item = sessions[SavedSessions.SelectedIndex];
                //Update DB
                dbHandler.DeleteSessions(item);
                //TODO Remove programs aswell.
                //Update view
                SavedSessions.Items.RemoveAt(index);

                //update controler
                sessions.RemoveAt(index);
                //TODO do it better https://msdn.microsoft.com/pl-pl/library/ms748365(v=vs.110).aspx? 
            }
            else if (OpenedPrograms.SelectedIndex != -1) //TODO if current session.
            {
                var index = OpenedPrograms.SelectedIndex;
                
                //update controler
                processes.RemoveAt(index);

                //update view
                OpenedPrograms.Items.RemoveAt(index);
            }
        }

        private void SavedSessions_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = SavedSessions.SelectedIndex;
            if (index != -1) //TODO Single responsibility principle. Change it. -1 - nothing selected.
            {               
                OpenedPrograms.Items.Clear();

                foreach (var program in sessions[index].listOfPrograms)
                {
                    OpenedPrograms.Items.Add(program.Name);
                }
            }
        }
    }
}
