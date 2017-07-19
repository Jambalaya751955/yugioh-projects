using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SQLite;

using CDBUpdater.Object;
using CDBUpdater.SQLite.CDB;

namespace CDBUpdater.SQLite
{
    class Commands
    {
        public static TResult ExecuteMethod<TResult, ParamType1, ParamType2>(string directory, Func<SQLiteConnection, ParamType1, ParamType2, TResult> Method, ParamType1 param1, ParamType2 param2)
        {
            TResult result;
            using (SQLiteConnection connection = new SQLiteConnection(@"Data Source=" + directory))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                    result = Method(connection, param1, param2);
                connection.Close();
            }
            return result;
        }

        public static TResult ExecuteMethod<TResult, ParamType>(string directory, Func<SQLiteConnection, ParamType, TResult> Method, ParamType param)
        {
            TResult result;
            using (SQLiteConnection connection = new SQLiteConnection(@"Data Source=" + directory))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                    result = Method(connection, param);
                connection.Close();
            }
            return result;
        }

        public static SQLiteCommand CreateCommand(string statement, SQLiteConnection connection)
        {
            return new SQLiteCommand
            {
                CommandText = statement,
                CommandType = CommandType.Text,
                Connection = connection
            };
        }

        public static bool ExecuteNonCommand(SQLiteCommand command)
        {
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string[]> ExecuteStringCommand(SQLiteCommand command, int columncount)
        {
            try
            {
                var values = new List<string[]>();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader[i].ToString());
                    }
                    values.Add(row.ToArray());
                }
                reader.Close();
                return values;

            }
            catch
            {
                return new List<string[]>();
            }
        }
    }
}
