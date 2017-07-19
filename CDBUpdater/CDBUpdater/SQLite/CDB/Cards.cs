using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using CDBUpdater.Object;
using CDBUpdater.Helpers;

namespace CDBUpdater.SQLite.CDB
{
    class Cards
    {
        public static bool InsertCard(SQLiteConnection connection, Card card, Data data)
        {
            try
            {
                List<string> insertSqls = new List<string>();

                for (int i = 0; i < card.IDs.Count; ++i)
                    insertSqls.Add(GetInsertSQL(card, data.IDs[i], i, false));

                List<SQLiteCommand> commands = new List<SQLiteCommand>();
                for (int i = 0; i < insertSqls.Count; ++i)
                    commands.Add(Commands.CreateCommand(insertSqls[i], connection));
                foreach (SQLiteCommand command in commands)
                    Commands.ExecuteNonCommand(command);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private enum T { id, name, desc }
        private enum D { id, ot, alias, setcode, type, atk, def, level, race, attribute, category }

        public static string GetInsertSQL(Card c, string id, int index, bool ignore)
        {
            int txtInx = 0;
            int datInx = 0;

            for (int i = 0; i < Load.cdbTexts.Count; ++i)
                if (id.TrimStart('0') == Load.cdbTexts[i][(int)T.id])
                    txtInx = i;

            for (int i = 0; i < Load.cdbDatas.Count; ++i)
                if (id.TrimStart('0') == Load.cdbDatas[i][(int)T.id])
                    datInx = i;

            string desc = c.CardText.IsEmpty() ? Load.cdbTexts[datInx][(int)T.desc] : GetCardDesc(c, datInx);
            if (c.Appearance == 0)
                c.Appearance = Convert.ToInt32(Load.cdbDatas[datInx][(int)D.ot]);

            StringBuilder st = new StringBuilder();
            if (ignore)
                st.Append("INSERT or ignore into datas values(");
            else
                st.Append("INSERT or replace into datas values(");
            st.Append(c.IDs[index].ToString()); st.Append(",");
            st.Append(c.Appearance); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.alias].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.setcode].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.type].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.atk].ToString()); ; st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.def].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.level].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.race].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.attribute].ToString()); st.Append(",");
            st.Append(Load.cdbDatas[datInx][(int)D.category].ToString()); st.Append(")");
            if (ignore)
                st.Append(";\nINSERT or ignore into texts values(");
            else
                st.Append(";\nINSERT or replace into texts values(");
            st.Append(c.IDs[index].ToString()); st.Append(",'");
            if (!c.Name.IsEmpty())
                st.Append(c.Name.Replace("'", "''")).Append("','");
            else
                st.Append(Load.cdbTexts[txtInx][(int)T.name].Replace("'", "''")).Append("','");
            st.Append(desc.Replace("'", "''"));
            for (int i = 3; i < 19; i++)
            {
                st.Append("','");
                st.Append(Load.cdbTexts[datInx][i].ToString().Replace("'", "''"));
            }
            st.Append("');");
            string sql = st.ToString();
            st = null;
            return sql;
        }

        public static string GetCardDesc(Card c, int datInx)
        {
            if (c.CardText == null)
                return null;

            if (!c.PendulumEffect.IsEmpty())
            {
                string level = Load.cdbDatas[datInx][(int)D.level];
                int scale = Convert.ToInt32(Load.cdbDatas[datInx][(int)D.level]) >> 0x10 & 0xff;
                List<string> types = FormatType(Int64.Parse(Load.cdbDatas[datInx][(int)D.type]));

                return Program.FormatString
                    .Replace("{PendScaleDesc}", Translations.PendulumScale[c.Language])
                    .Replace("{PendScale}", scale.ToString())
                    .Replace("{PendEffectDesc}", Translations.PendulumEffect[c.Language])
                    .Replace("{PendEffect}", c.PendulumEffect)
                    .Replace("{CardTextDesc}", types.Contains("Normal") ?
                        Translations.FlavorText[c.Language] :
                        Translations.MonsterEffect[c.Language])
                    .Replace("{CardText}", c.CardText);
            }
            else
                return c.CardText;
        }

        private static List<string> FormatType(long type)
        {
            List<string> types_l = new List<string>();
            uint filter = 1;

            for (int i = 0; filter != 0x2000000; filter <<= 1, ++i)
            {
                string type_s = Types.GetValue(type & filter);
                if (type_s != null)
                    types_l.Add(type_s);
            }

            return types_l;
        }

        public static string GetCreateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"CREATE TABLE texts(id integer primary key,name text,desc text");
            for (int i = 1; i <= 16; i++)
            {
                sb.Append(",str");
                sb.Append(i.ToString());
                sb.Append(" text");
            }
            sb.Append(");");
            sb.Append(@"CREATE TABLE datas(");
            sb.Append("id integer primary key,ot integer,alias integer,");
            sb.Append("setcode integer,type integer,atk integer,def integer,");
            sb.Append("level integer,race integer,attribute integer,category integer) ");
            return sb.ToString();
        }
    }
}
