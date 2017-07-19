using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

using CDBUpdater.Object;

namespace CDBUpdater.CardSearch.Wikia
{
    partial class Set
    {
        private static bool IsCardReleased(List<Card.Pack> packs)
        {
            foreach (var pack in packs)
            {
                string releaseDate = '-' + pack.ReleaseDate;
                string[] tmp = new string[3] { "", "", "" };
                for (int i = 0, c = 0; i < releaseDate.Length; ++i)
                    if (releaseDate[i] == '-' && c < tmp.Length)
                        tmp[++c - 1] += releaseDate[++i];
                    else if (c > 0)
                        tmp[c - 1] += releaseDate[i];
                int[] packDate = new int[3] { int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]) };
                int[] currDate = new int[] { DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day };
                if (packDate[0] - currDate[0] <= 0 && packDate[1] - currDate[1] <= 0 && packDate[2] - currDate[2] <= 0)
                    return true;
            }
            return false;
        }

        public static int Appearance(ref HtmlDocument cardPage, List<Card.Pack> packs, string language)
        {
            bool isOcg = language.ToLower() == "ja";
            string otherLang = isOcg ? "en" : "ja";
            List<Card.Pack> packsOther = Packs(ref cardPage, Get.Value(otherLang), otherLang);

            bool isReleasedOther = IsCardReleased(Packs(ref cardPage, Get.Value(otherLang), otherLang));
            bool isReleasedThis = IsCardReleased(packs);

            return isReleasedThis || isReleasedOther ? ((isOcg ? isReleasedThis : isReleasedOther) ? 1 : 0) + ((isOcg ? isReleasedOther : isReleasedThis) ? 2 : 0) :
                ((isOcg ? packs.Count < 1 : packsOther.Count < 1) ? 1 : 0) + ((isOcg ? packsOther.Count < 1 : packs.Count < 1) ? 1 : 0);
        }

        /*public static int Appearance(ref HtmlDocument cardPage)
        {
            if (cardPage == null)
                return 0;

            var ocgNode = cardPage.DocumentNode.SelectSingleNode("//td[@class='cardtablespanrow']//a[@title='Yu-Gi-Oh! Official Card Game']");
            var tcgNode = cardPage.DocumentNode.SelectSingleNode("//td[@class='cardtablespanrow']//a[@title='Yu-Gi-Oh! Trading Card Game']");
            
            return (ocgNode != null ? 1 : 0) + (tcgNode != null ? 2 : 0);
        }*/

        /*
        public static int Appearance(ref HtmlDocument cardPage)
        {
            if (cardPage == null)
                return 0;

            var tableNodes = cardPage.DocumentNode.SelectNodes("//td[@class='cardtablerowdata']");
            string appearance = "";

            foreach (var row in tableNodes)
            {
                string content = row.InnerText.ToLower();
                if (content.Contains("unlimited") || content.Contains("semi-limited") || content.Contains("forbidden"))
                    if (content.Contains("ocg")) appearance += "|ocg|";
                    else if (content.Contains("tcg")) appearance += "|tcg|";
                    else appearance += "|ocg|tcg|";
            }

            return (appearance.Contains("ocg") ? 1 : 0) + (appearance.Contains("tcg") ? 2 : 0);
        }
        */
    }
}
