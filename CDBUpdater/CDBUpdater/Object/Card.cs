using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Object
{
    class Card
    {
        public string Name { get; set; }
        public string CardText { get; set; }
        public string PendulumEffect { get; set; }
        public int Appearance { get; set; }
        public string Language { get; set; }
        public List<string> IDs = new List<string>();
        public List<Pack> Packs = new List<Pack>();

        public class Pack
        {
            public string PackName { get; set; }
            public string ReleaseDate { get; set; }
            public string CardID { get; set; }
            public string Rarity { get; set; }
        }
    }
}
