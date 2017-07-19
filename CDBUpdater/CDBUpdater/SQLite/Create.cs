using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace CDBUpdater.SQLite
{
    class Create
    {
        public static int CDB(string directory, params string[] SQLs)
        {
            if (File.Exists(directory) || SQLs == null)
                return 0;

            int result = 0;
            SQLiteConnection.CreateFile(directory);
            using (SQLiteConnection connection = new SQLiteConnection(@"Data Source=" + directory))
            {
                connection.Open();
                using (SQLiteTransaction trans = connection.BeginTransaction())
                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(connection))
                            foreach (string SQLstr in SQLs)
                            {
                                cmd.CommandText = SQLstr;
                                result += cmd.ExecuteNonQuery();
                            }
                    }
                    catch
                    {
                        trans.Rollback();
                        result = -1;
                    }
                    finally
                    {
                        trans.Commit();
                    }
                connection.Close();
            }
            return result;
        }
    }
}
