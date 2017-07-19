using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Object
{
    class Input
    {
        public LoadSettings Load = new LoadSettings();

        public string CdbPath { get; set; }
        public string SavingDirectory { get; set; }
        public string FormatDirectory { get; set; }
        public string Language { get; set; }

        public List<string[]> Packs { get; set; }
        public string PackPath { get; set; }
        public int Months { get; set; }
    }

    class LoadSettings
    {
        public bool All { get; set; }

        public bool Wikia { get; set; }
        public bool YGODB { get; set; }

        public bool Name { get; set; }
        public bool Desc { get; set; }
        public bool Ot { get; set; }
        public bool Pack { get; set; }
    }
}
