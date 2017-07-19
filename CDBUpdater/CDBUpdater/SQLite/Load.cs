using CDBUpdater.Helpers;
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CDBUpdater.SQLite
{
    public class CDBName
    {
        public string Name { get; set; }
        public List<int> ID = new List<int>();

        public CDBName(string name, int id = -1)
        {
            Name = name;
            if (id > -1)
                ID.Add(id);
        }
    }

    class Load
    {
        private enum T { id, name, desc }

        public static List<string[]> LoadData(SQLiteConnection connection)
        {
            SQLiteCommand datacommand = new SQLiteCommand("SELECT id, ot, alias, setcode, type, atk, def, level, race, attribute, category FROM datas", connection);
            return Commands.ExecuteStringCommand(datacommand, 11);
        }

        public static List<string[]> LoadText(SQLiteConnection connection)
        {
            SQLiteCommand textcommand = new SQLiteCommand("SELECT id, name, desc, str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11, str12, str13, str14, str15, str16 FROM texts", connection);
            return Commands.ExecuteStringCommand(textcommand, 19);
        }

        public static List<string[]> cdbDatas = new List<string[]>();
        public static List<string[]> cdbTexts = new List<string[]>();

        public static List<CDBName> GetAllCards(string cdbDirectory)
        {
            if (!File.Exists(cdbDirectory))
                return null;

            SQLiteConnection connection = new SQLiteConnection("Data Source=" + cdbDirectory);
            try
            {
                connection.Open();
                cdbTexts = LoadText(connection);
                cdbDatas = LoadData(connection);
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return null;
            }

            List<CDBName> cdbNames = new List<CDBName>();
            int loadingStep = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < cdbTexts.Count; ++i)
            {
                if (cdbNames != null && cdbTexts[i] != null && (int)T.name < cdbTexts[i].Length && cdbTexts[i][(int)T.name] != null
                    && (int)T.id < cdbTexts[i].Length && cdbTexts[i][(int)T.id] != null)
                    cdbNames.Add(new CDBName(cdbTexts[i][(int)T.name], Convert.ToInt32(cdbTexts[i][(int)T.id])));

                PrintLoadingState(sw.ElapsedMilliseconds, ++loadingStep, cdbTexts.Count);
            }

            List<CDBName> cdbNames2 = new List<CDBName>();
            List<string> multiple = new List<string>();

            for (int i = 0; i < cdbNames.Count; ++i)
            {
                PrintLoadingState(sw.ElapsedMilliseconds, ++loadingStep, cdbTexts.Count);

                if (multiple.Contains(cdbNames[i].Name.ToLower()))
                    continue;
                CDBName cdbName = new CDBName(cdbNames[i].Name);

                for (int j = 0; j < cdbNames.Count; ++j)
                    if (cdbNames[j].Name == cdbName.Name)
                        for (int h = 0; h < cdbNames[j].ID.Count; ++h)
                            cdbName.ID.Add(cdbNames[j].ID[h]);

                if (cdbName.ID.Count > 1)
                    multiple.Add(cdbName.Name.ToLower());

                cdbNames2.Add(cdbName);
            }

            sw.Stop();
            sw.Reset();
            return cdbNames2;
        }

        private static void PrintLoadingState(long elapsed, int step, int count)
        {
            double elapsedTime = Convert.ToDouble(elapsed) / 1000.0;
            string percentage = (Convert.ToDouble(step) / count * 50).ToString("0");
            Console.SetCursorPosition(0, 1);
            Console.Write(" Loading CDB: {0}%{1} [{2}s]\n",
                percentage, String.Concat(Enumerable.Repeat(" ", 3 - percentage.Length)),
                elapsedTime.ToString().Replace(',', '.').Substring(0, elapsedTime.ToString().IndexOf(',') + 2));
        }

        public static List<string[]> Table(SQLiteConnection connection, string table, string[] columns)
        {
            if (columns.Length < 1)
                return null;

            string sql = "SELECT " + columns[0];
            for (int i = 1; i < columns.Length; ++i)
                sql += ", " + columns[i];
            sql += " FROM " + table;

            SQLiteCommand datacommand = new SQLiteCommand(sql, connection);
            return Commands.ExecuteStringCommand(datacommand, columns.Length);
        }
    }
}
