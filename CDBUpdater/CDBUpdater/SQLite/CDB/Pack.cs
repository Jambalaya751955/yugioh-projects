using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using CDBUpdater.Object;

namespace CDBUpdater.SQLite.CDB
{
    class Pack
    {
        public static bool InsertPack(SQLiteConnection connection, List<Card.Pack> Packs, List<string> ids)
        {
            try
            {
                SQLiteCommand command = Commands.CreateCommand("INSERT INTO pack VALUES(@id,@pack_id,@pack,@rarity,@date);", connection);

                foreach (string id in ids)
                {
                    foreach (Card.Pack pack in Packs)
                    {
                        command.Parameters.Add(new SQLiteParameter("@id", id));
                        command.Parameters.Add(new SQLiteParameter("@pack_id", pack.CardID));
                        command.Parameters.Add(new SQLiteParameter("@pack", pack.PackName));
                        command.Parameters.Add(new SQLiteParameter("@rarity", pack.Rarity));
                        command.Parameters.Add(new SQLiteParameter("@date", pack.ReleaseDate));

                        var parameters = new List<SQLiteParameter>();
                        command.Parameters.AddRange(parameters.ToArray());
                        Commands.ExecuteNonCommand(command);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetCreateSQL()
        {
            return "CREATE TABLE pack(id integer,pack_id text,pack text,rarity text,date text);";
        }
    }
}
