using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace DatabaseDownloader.Wikia_
{
    partial class Set
    {
        public static string Name(ref HtmlDocument cardPage, int langNum)
        {
            if (cardPage == null)
                return null;

            var languageTable = cardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//tr[@class='cardtablerow']//th[@class='cardtablerowheader']");
            var cardNameTable = cardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//tr[@class='cardtablerow']//td[@class='cardtablerowdata']");

            if (languageTable == null || cardNameTable == null)
                return null;

            string name = null;
            int langLine = -1;

            for (int i = 0; i < languageTable.Count; ++i)
            {
                string rowLang = languageTable[i].InnerText.ToLower();
                if (Get.languages.Contains(rowLang) && (rowLang == Get.languages[langNum]
                    || ((langNum == 5) ? (rowLang == Get.languages[6]) : false)))
                    langLine = i;
            }

            for (int i = 0; i < languageTable.Count; ++i)
            {
                string rowText = "";
                if (i == langLine)
                    rowText = Regex.Replace(cardNameTable[i].InnerText, @"\t|\n|\r|Check translation", string.Empty);
                if (i == langLine) name = HttpUtility.HtmlDecode(rowText);
            }

            return name;
        }
    }
}
