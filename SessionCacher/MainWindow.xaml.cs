using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SessionCacher.Annotations;
using SessionCacher.Controlers;

namespace SessionCacher
{
    public partial class MainWindow : MetroWindow
    {
        private List<Process> processes;
        private DBHandler dbHandler;
        private List<Session> sessions;
        private Session undoSession;
        //TODO OBSERVABLE COLLECTION + DATABINDING!
        public MainWindow()
        {
            InitializeComponent();
            dbHandler = new DBHandler();
            refreshSessions(); 
        }

        public void refreshSessions()
        {
            sessions = new List<Session>();
            CurrentSession.DataContext = GetCurrentSession();
            
            sessions.AddRange(getSessionsFromDB());
            SavedSessions.ItemsSource = sessions;
            refreshCurrentSession();
        }

        public List<Session> getSessionsFromDB()
        {
            return dbHandler.GetSessionsWithProgramList();
        }

        public Session GetCurrentSession()
        {   
            //Get all procesees
            processes = Process.GetProcesses().ToList();

            //Removes all procceses that violates the restrictions //TODO workaround privilages.
            processes.RemoveAll(Restrictions.CheckRestrictions);

            //GetActiveTabUrl();
     
            return new Session("Current session", (processes));
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
            }
            await this.ShowMessageAsync("Success", "You will find your session on session list");
            refreshSessions();
        }

        private void refreshCurrentSession()
        {
            OpenedPrograms.ItemsSource = GetCurrentSession().listOfPrograms;
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
            OpenedPrograms.ItemsSource = sessions[index].listOfPrograms;
        }

        private async void Revert_OnClick(object sender, RoutedEventArgs e)
        {
            var id = dbHandler.InsertSessionToTable(undoSession);
            //add to session
            foreach (var program in undoSession.listOfPrograms)
            {
                dbHandler.InsertProgramToTable(program);

                //TODO process.MainModule.FileName get privilages.
                //TODO Get Arguments
            }
            await this.ShowMessageAsync("Success", "You will find your session on session list");
            refreshSessions();
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

        private void CurrentSession_OnClick(object sender, RoutedEventArgs e)
        {
            refreshCurrentSession();
        }
    }
}
