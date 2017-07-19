using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;

using CDBUpdater.Helpers;
using CDBUpdater.Object;

namespace CDBUpdater.CardSearch.YugiohDB
{
    partial class Set
    {
        public static List<Card.Pack> Packs(ref HtmlDocument cardPage, string language)
        {
            List<Card.Pack> Sets = new List<Card.Pack>();

            HtmlNodeCollection parent = cardPage.DocumentNode.SelectNodes("//tr[@class='row']");
            if (parent == null)
                return Sets;

            foreach (HtmlNode childs in parent)
            {
                if (childs.InnerText.Contains("[") || childs.InnerText.Contains("]"))
                    continue;

                Card.Pack Set = new Card.Pack();

                // Get Release Date
                HtmlNode rd = childs.SelectSingleNode(".//td[@class='t_center']");
                if (rd != null)
                    Set.ReleaseDate = rd.InnerText.HtmlDecode();

                // Get Pack Name
                HtmlNode pn = childs.SelectSingleNode(".//td//b");
                if (pn != null)
                    Set.PackName = new CultureInfo("fr", false).TextInfo.ToTitleCase(pn.InnerText.ToLower().HtmlDecode().ToLower());

                // Get Card Id
                bool set = false;
                HtmlNodeCollection cid = childs.SelectNodes(".//td");
                if (cid != null)
                    foreach (HtmlNode child in cid)
                        if (set)
                        {
                            Set.CardID = child.InnerText.HtmlDecode();
                            break;
                        }
                        else set = true;

                // Get Rarity
                HtmlNode ra = childs.SelectSingleNode(".//td//img");
                if (ra != null)
                    Set.Rarity = ra.Attributes["alt"].Value.HtmlDecode();
                else
                    switch (language.ToLower())
                    {
                        case "en": Set.Rarity = "Common"; break;
                        case "de": Set.Rarity = "Common"; break;
                        case "it": Set.Rarity = "Comune"; break;
                        case "es": Set.Rarity = "Común"; break;
                        case "fr": Set.Rarity = "Commune"; break;
                        case "ja": Set.Rarity = "共通"; break;

                    }

                // Add SetDetails to Card Object
                Sets.Add(Set);
            }

            return Sets;
        }
    }
}
