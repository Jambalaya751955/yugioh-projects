using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Snrk.SQLite
{
    public static class SQLite
    {
        public static void CreateDatabase(string path)
        {
            System.Data.SQLite.SQLiteConnection.CreateFile(path);
        }
    }

    public sealed class SQLiteConnection : IDisposable
    {
        public readonly string Path;

        private System.Data.SQLite.SQLiteConnection m_Connection;

        public SQLiteConnection()
        {
            Path = null;
            m_Connection = null;
        }

        public SQLiteConnection(string path)
        {
            Path = path;
            if (!File.Exists(path))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(path);
            }
        }

        ~SQLiteConnection() { Dispose(); }
        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            Close();
            m_Connection = new System.Data.SQLite.SQLiteConnection("Data Source = " + this.Path);
            m_Connection.Open();
        }

        public void Close()
        {
            if (m_Connection != null)
            {
                m_Connection.Close();
                m_Connection.Dispose();
                m_Connection = null;
            }
        }
        
        public IEnumerable<Dictionary<string, string>> ReadRows(string table, IEnumerable<string> columns)
        {
            foreach (Dictionary<string, string> row in ReadRows(null, table, columns))
            {
                yield return row;
            }
        }

        public IEnumerable<Dictionary<string, string>> ReadRows(string condition, string table, IEnumerable<string> columns)
        {
            if (m_Connection == null || m_Connection.State == ConnectionState.Closed || String.IsNullOrEmpty(table) || columns == null)
                yield break;
            
            string query = "SELECT ";
            foreach (var column in columns)
                query += (query.Length > 7 ? ", " : "") + column;
            query += " FROM " + table + (!String.IsNullOrEmpty(condition) ? " WHERE " + condition
                .Replace("where", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim() : "") + ";";
            
            SQLiteCommand command = new SQLiteCommand(query, m_Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, string>();
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    row.Add(reader.GetName(i), reader[i].ToString());
                }
                yield return row;
            }
            reader.Close();
        }

        public void CreateTable(string name, IEnumerable<string> columns, bool @override = true)
        {
            if (m_Connection == null || m_Connection.State == ConnectionState.Closed || String.IsNullOrEmpty(name))
                return;

            string columnString = "";
            foreach (string column in columns)
                columnString += (columnString.Length > 0 ? ", " : "") + column;
            string sqlString = (@override ? "DROP TABLE IF EXISTS " + name + "; CREATE TABLE " : "CREATE TABLE IF NOT EXISTS ") +
                name + "(" + columnString + ");";
            
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
                try
                {
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Commit();
                }
        }

        public void InsertValues(string table, IEnumerable<string> values, bool replace = false)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT OR ");
            sb.Append(replace ? "REPLACE" : "IGNORE");
            sb.Append(" INTO ");
            sb.Append(table);
            sb.Append(" VALUES(");
            
            foreach (string value in values)
            {
                if (value == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    bool isNumber = value.Length > 0;
                    foreach (char c in value)
                        if (!Char.IsNumber(c))
                        {
                            isNumber = false;
                            break;
                        }
                    if (!isNumber)
                        sb.Append("'");
                    sb.Append(value.Replace("'", "''"));
                    if (!isNumber)
                        sb.Append("'");
                }
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            string sql = sb.ToString();

            sb.Clear();
            sb = null;
            
            using (var command = m_Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
    }
}
