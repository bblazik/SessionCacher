using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SessionCacher
{
    class DBHandler
    {
        private SQLiteConnection conn;
        private SQLiteCommand cmd;

        public DBHandler()
        {
            if (!System.IO.File.Exists("user-data.db3"))
                SQLiteConnection.CreateFile("user-data.db3");

            conn = new SQLiteConnection("data source=user-data.db3");
            cmd = new SQLiteCommand(conn);
            conn.Open();

            string createQuery = @"CREATE TABLE IF NOT EXISTS
                                  [Session](
                                  [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  [SessionName] NVARCHAR(2048) NULL
                                  )";
            cmd.CommandText = createQuery;
            cmd.ExecuteNonQuery();

            createQuery = @"CREATE TABLE IF NOT EXISTS
                                  [Program](
                                  [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  [Name] NVARCHAR(2048) NULL,
                                  [Path] NVARCHAR(2048) NULL,
                                  [Arguments] NVARCHAR(2048) NULL,
                                  [SessionId] INTEGER NOT NULL,
                                  FOREIGN KEY(SessionId) REFERENCES SESSION(Id)
                                  )";
            cmd.CommandText = createQuery;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public int InsertSessionToTable(Session session)
        {
            conn.Open();

            cmd.CommandText = "INSERT INTO Session (SessionName) values('" + session.Name + "')" ; //parameters (Name,Gender). // values ('alex', 'male'); 
            cmd.ExecuteNonQuery();

            var id = conn.LastInsertRowId;

            conn.Close();
            return (int)id;
        }

        public void InsertProgramToTable(Program program)
        {
            conn.Open();

            cmd.CommandText = "INSERT INTO Program (Name, Path, Arguments, SessionId) values('"+ program.GetValues() +"')"; //parameters (Name,Gender). // values ('alex', 'male'); 
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public List<Program> ReadProgramsFromSession(Session session)
        {
            bool closeFlag = true;
            var programList = new List<Program>();
            if (conn.State == ConnectionState.Closed) conn.Open();
            else closeFlag = false;

            cmd.CommandText = "SELECT * from Session join Program on Session.id == Program.SessionId and Session.Id == " + session.Id;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                programList.Add(new Program(reader["Name"].ToString(), reader["Path"].ToString(), reader["Arguments"].ToString()));
            }
            if(closeFlag)
                conn.Close();   
            return programList;
        }

        public List<Session> GetReadSessions()
        {
            var sessionList = new List<Session>();
            conn.Open();

            cmd.CommandText = "SELECT * from Session";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sessionList.Add(new Session(Convert.ToInt32(reader["Id"]), reader["SessionName"].ToString()));
            }

            conn.Close();
            return sessionList;
        }

        public List<Session> GetSessionsWithProgramList()
        {
            var sessionList = new List<Session>();
            conn.Open();

            cmd.CommandText = "SELECT * from Session";
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var session = new Session();
                session = new Session( Convert.ToInt32(reader["Id"]), reader["SessionName"].ToString());
                sessionList.Add(session);
            }
            reader.Close();

            foreach (var session in sessionList)
            {
                var programList = new List<Program>();
                cmd.CommandText = "SELECT * from Session join Program on Session.id == Program.SessionId and Session.Id == " + session.Id;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    programList.Add(new Program(reader["Name"].ToString(), reader["Path"].ToString(), reader["Arguments"].ToString()));
                }
                reader.Close();
                session.listOfPrograms = programList;
            }
            

            
            


            conn.Close();
            return sessionList;
        }

        public void DeleteSessions(Session session)
        {
            conn.Open();

            //Delete session
            cmd.CommandText = "DELETE FROM Session Where Id == "+ session.Id ; //parameters (Name,Gender). // values ('alex', 'male'); 
            cmd.ExecuteNonQuery();

            //Delete related programs
            cmd.CommandText = "DELETE FROM Program Where SessionId == " + session.Id; //parameters (Name,Gender). // values ('alex', 'male'); 
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
