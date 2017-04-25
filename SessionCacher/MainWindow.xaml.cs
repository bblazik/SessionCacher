using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Migrations.History;
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
using System.Text.RegularExpressions;
using System.Windows.Automation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SessionCacher.Controlers;

namespace SessionCacher
{
    public partial class MainWindow : MetroWindow
    {
        private List<Process> processes;
        private DBHandler dbHandler;
        private List<Session> sessions;
        private Session currentSession;
        private Session undoSession;

        public MainWindow()
        {
            InitializeComponent();
            dbHandler = new DBHandler();
            //TODO initilize ALL LIST
            refreshSessions();

            //TODO ADD REVERT ACTION.            
        }

        public void refreshSessions()
        {
            sessions = new List<Session>();
            sessions.Add(GetCurrentSession());
            sessions.AddRange(getSessionsFromDB());
            addToSessionsTab(sessions);
            SavedSessions.SelectedIndex = 0;
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
                SavedSessions.Items.Add(session.name);
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

        public Session GetCurrentSession()
        {   //Get all procesees
            processes = Process.GetProcesses().ToList();

            //Removes all procceses that violates the restrictions
            processes.RemoveAll(Restrictions.CheckRestrictions);

            //GetActiveTabUrl();
           //var his = 

            return currentSession = new Session("Current session", (processes));
        }

        public static string GetActiveTabUrl()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");

            if (procsChrome.Length <= 0)
                return null;

            foreach (Process proc in procsChrome)
            {
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                // to find the tabs we first need to locate something reliable - the 'New Tab' button 
                AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                var SearchBar = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                if (SearchBar != null)
                    return (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            }

            return null;
        }

        private async void SaveSession(Session session)
        {
        
            session.name = await this.ShowInputAsync("Title", "enter some text");


            var id = dbHandler.InsertSessionToTable(session);
            //add to session
            foreach (var process in processes)
            {
                dbHandler.InsertProgramToTable(new Program(process.MainWindowTitle, process.MainModule.FileName, process.StartInfo.Arguments, id));
                
                //TODO process.MainModule.FileName get privilages.
                //TODO Get Arguments
                //TODO success dialog.    
            }
            await this.ShowMessageAsync("Success", "You will find your session on session list");
            refreshSessions();
        }

        // EVENTS.

        private void button_Click(object sender, RoutedEventArgs e)
        {
            refreshSessions();
        }

        private void save_Click_1(object sender, RoutedEventArgs e)
        {
            SaveSession(new Session());
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (SavedSessions.SelectedIndex != -1 && SavedSessions.SelectedIndex != 0)
            {
                var index = SavedSessions.SelectedIndex;
                var item = sessions[SavedSessions.SelectedIndex];
                undoSession = item;
                //Update DB
                dbHandler.DeleteSession(item);

                //Update view
                SavedSessions.Items.RemoveAt(index);

                //update controler
                sessions.RemoveAt(index);
                //TODO do it better https://msdn.microsoft.com/pl-pl/library/ms748365(v=vs.110).aspx? 
            }
            else if (OpenedPrograms.SelectedIndex != -1) //TODO if current session.
            {
                var index = OpenedPrograms.SelectedIndex;
                var sessionIndex = SavedSessions.SelectedIndex;

                //update controler
                processes.RemoveAt(index);

                //update view
                OpenedPrograms.Items.RemoveAt(index);
            }
        }

        private void SavedSessions_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = SavedSessions.SelectedIndex;
            if (index == -1) return;

            OpenedPrograms.Items.Clear();
            foreach (var program in sessions[index].listOfPrograms)
            {
                OpenedPrograms.Items.Add(program.Name);
            }
        }

        private void Revert_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSession(undoSession);
        }

        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            var index = SavedSessions.SelectedIndex;
            var session = sessions[index];
            foreach (var program in session.listOfPrograms)
            {
                try
                {
                    Process.Start(program.Path, program.Arguments ?? "");
                }
                catch (Exception)
                {                    
                    throw;
                }
                
            }
            
        }
    }
}
