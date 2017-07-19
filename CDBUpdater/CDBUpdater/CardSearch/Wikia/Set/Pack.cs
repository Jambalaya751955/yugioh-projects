using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

using CDBUpdater.Object;
using CDBUpdater.Helpers;

namespace CDBUpdater.CardSearch.Wikia
{
    partial class Set
    {
        public static List<Card.Pack> Packs(ref HtmlDocument cardPage, int langNum, string language)
        {
            List<Card.Pack> Packs = new List<Card.Pack>();

            if (cardPage == null || language.IsEmpty())
                return Packs;
            
            /// Get Sets by Language

            var allTables = cardPage.DocumentNode.SelectNodes("//tr[@class='cardtablerow']//td[@class='cardtablespanrow']");

            if (allTables != null)
                foreach (var textTable in allTables)
                    // Check if table contains TCG or OCG sets
                    if (textTable.SelectSingleNode(".//a[@title='Yu-Gi-Oh! Trading Card Game']") != null
                        || textTable.SelectSingleNode(".//a[@title='Yu-Gi-Oh! Official Card Game']") != null)
                    {
                        // TCG or OCG sets are given. Now check if Sets are existant in card language
                        var setLangTable = textTable.SelectNodes(".//table[@class='collapsible autocollapse navbox-inner']");

                        if (setLangTable != null)
                            foreach (var setLang in setLangTable)
                            {
                                var langNode = setLang.SelectSingleNode(".//div[@style='font-size: 110%;']");
                                if (langNode == null)
                                    continue;
                                string lang = langNode.InnerText.ToLower();
                                if (lang != null && (lang.Contains(Get.languages[langNum])
                                    || (langNum == 0 ? lang.Contains(Get.languages[langNum] + "—worldwide") : false)))
                                {
                                    // Sets are existant in the Card's Language
                                    var setLine = setLang.SelectNodes(".//table[@class='wikitable sortable card-list cts']//tr");
                                    if (setLine != null)
                                    {
                                        bool isSet = false;
                                        foreach (var setInfo in setLine)
                                        {
                                            if (isSet)
                                            {
                                                var detailedInfo = setInfo.SelectNodes(".//td");

                                                if (detailedInfo != null)
                                                {
                                                    Card.Pack Set = new Card.Pack();
                                                    for (int i = 0; i < detailedInfo.Count; ++i)
                                                    {
                                                        string content = Translate.CardRarity(detailedInfo[i].InnerText.Trim(), language);
                                                        
                                                        if (i == 0)
                                                            Set.ReleaseDate = content;
                                                        else if (i == 1)
                                                            Set.CardID = content;
                                                        else if (i == 2 && (lang.Contains(Get.languages[5]) || langNum == 0))
                                                            Set.PackName = content;
                                                        else if (i == 3 && langNum == 0)
                                                            Set.Rarity = content;
                                                        else if (i == 3 && langNum > 0)
                                                            Set.PackName = content;
                                                        else if (i == 4 && (lang.Contains(Get.languages[5]) || langNum > 0))
                                                            Set.Rarity = content;
                                                    }
                                                    Packs.Add(Set);
                                                }
                                            }
                                            else isSet = true;
                                        }
                                    }
                                }
                            }
                    }

            return Packs;
        }
    }
}
