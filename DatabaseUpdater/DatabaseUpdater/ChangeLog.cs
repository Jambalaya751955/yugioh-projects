using System;
using System.Collections.Generic;

namespace DatabaseUpdater
{
    class Changelog
    {
        public string DatabaseName { get; set; }
        public List<Row> Changes { get; set; }

        public Changelog(string databaseName)
        {
            this.DatabaseName = databaseName;
            this.Changes = new List<Row>();
        }

        ~Changelog() { Clear(); }
        public void Clear()
        {
            if (Changes != null)
            {
                for (int i = 0; i < Changes.Count; ++i)
                    Changes[i].Clear();
                this.Changes.Clear();
                this.Changes = null;
            }
        }

        public override string ToString()
        {
            string message = "";
            foreach (Row row in this.Changes)
            {
                if (row.Count < 1)
                    continue;
                message += Environment.NewLine + row.Identifier + ": ";
                for (int i = 0; i < row.Count; ++i)
                {
                    message += (i > 0 ? "," : "") + row[i][0] + (i == row.Count - 1 ? "." : "");
                }
            }
            return message.TrimStart();
        }

        public class Row
        {
            public string Identifier { get; set; }
            public int Count
            {
                get
                {
                    return m_Changes.Count;
                }
            }

            private List<string[]> m_Changes;

            public Row()
            {
                m_Changes = new List<string[]>();
            }

            ~Row() { Clear(); }
            public void Clear()
            {
                if (m_Changes != null)
                {
                    m_Changes.Clear();
                    m_Changes = null;
                }
            }

            public string[] this[int index]
            {
                get
                {
                    return m_Changes[index];
                }
            }

            public void Add(string change, string addition = null)
            {
                m_Changes.Add(new[] { change, addition });
            }
        }
    }
}
