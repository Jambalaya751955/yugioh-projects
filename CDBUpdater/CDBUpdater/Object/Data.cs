using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Object
{
    class Data
    {
        public string searchID { get; set; }
        public string language { get; set; }
        public string cdbName { get; set; }
        public List<string> IDs { get; set; }

        public string encodedName { get; set; }
        public string ygodbUrl { get; set; }

        public Data(List<string> IDs, string searchID, string language, string cdbName)
        {
            this.IDs = IDs;
            this.searchID = searchID;
            this.language = language;
            this.cdbName = cdbName;
        }
    }
}
