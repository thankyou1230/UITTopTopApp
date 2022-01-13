using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TOPTOP
{
    public class LocalDatabase
    {
        static string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public bool CreateLocalDatabase()
        {
            try
            {
                string path = System.IO.Path.Combine(folder, "local.db");
                SQLiteConnection connection = new SQLiteConnection(path);
                connection.CreateTable<CurrentUser>();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool AddCurrentUser(CurrentUser user)
        {
            try
            {
                string path = System.IO.Path.Combine(folder, "local.db");
                SQLiteConnection connection = new SQLiteConnection(path);
                int _ = connection.Insert(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool EmptyAllCurrentUsers()
        {
            try
            {
                string path = System.IO.Path.Combine(folder, "local.db");
                SQLiteConnection connection = new SQLiteConnection(path);
                int _ = connection.DeleteAll<CurrentUser>();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public CurrentUser GetCurrentUser()
        {
            try
            {
                string path = System.IO.Path.Combine(folder, "local.db");
                SQLiteConnection connection = new SQLiteConnection(path);
                CurrentUser user = connection.Table<CurrentUser>().FirstOrDefault();
                return user;
            }
            catch
            {
                return null;
            }
        }
        public bool RemoveCurrentUser(CurrentUser user)
        {
            try
            {
                string path = System.IO.Path.Combine(folder, "local.db");
                SQLiteConnection connection = new SQLiteConnection(path);
                int _ = connection.Delete<CurrentUser>(user.email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
