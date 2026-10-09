using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace PersonalSpendingTracker
{
    public static class Database
    {
        static string connectionString;

        public static void Init()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PersonalSpendingTracker");
            Directory.CreateDirectory(folder);
            connectionString = "Data Source=" + Path.Combine(folder, "data.db");

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Subscriptions " +
                             "(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Price REAL, IsYearly INTEGER)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Subscription> Load(bool yearly)
        {
            var list = new List<Subscription>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Name, Price FROM Subscriptions WHERE IsYearly = @y";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@y", yearly ? 1 : 0);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Subscription
                            {
                                Id = r.GetInt32(0),
                                Name = r.GetString(1),
                                Price = Convert.ToDecimal(r.GetDouble(2))
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static int Add(Subscription s, bool yearly)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Subscriptions (Name, Price, IsYearly) VALUES (@n, @p, @y); " +
                             "SELECT last_insert_rowid()";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@n", s.Name);
                    cmd.Parameters.AddWithValue("@p", (double)s.Price);
                    cmd.Parameters.AddWithValue("@y", yearly ? 1 : 0);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Subscriptions WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}