using System;
using HtmlAgilityPack;

namespace DatabaseDownloader.Wikia_
{
    partial class Set
    {
        public static string[] Effect(ref HtmlDocument cardPage, string language)
        {
            string[] Effect = new string[2] { null, null };

            if (cardPage == null || String.IsNullOrEmpty(language))
                return Effect;

            language = language.ToLower();

            var a = cardPage.DocumentNode.SelectNodes(
                "//tr//td//table[@class='collapsible " +
                ((language == "en") ? "expanded" : "autocollapse") +
                " navbox-inner']//tr//td[@class='navbox-list']//dl//dt" +
                ((language == "en") ? "" : "//span[@lang='" + language + "']"));

            var b = cardPage.DocumentNode.SelectNodes(
                "//tr//td//table[@class='collapsible " +
                ((language == "en") ? "expanded" : "autocollapse") +
                " navbox-inner']//tr//td[@class='navbox-list']//dl//dd" +
                ((language == "en") ? "" : "//span[@lang='" + language + "']"));

            if (a != null && b != null)
            {
                for (int i = 0; i < b.Count; ++i)
                    if (i == 0)
                        Effect[1] = b[i].InnerHtml.Replace("<br>", "\n").Trim().RemoveBetween('<', '>');
                    else if (i == 1)
                        Effect[0] = b[i].InnerHtml.Replace("<br>", "\n").Trim().RemoveBetween('<', '>');
            }
            else
            {
                var tmp = cardPage.DocumentNode.SelectSingleNode(
                    "//tr//td//table[@class='collapsible " +
                    ((language == "en") ? "expanded" : "autocollapse") +
                    " navbox-inner']//tr//td[@class='navbox-list']" +
                    ((language == "en") ? "" : "//span[@lang='" + language + "']"));

                if (tmp != null)
                {
                    Effect[0] = tmp.InnerHtml.Replace("<br>", "\n").Trim().RemoveBetween('<', '>');
                }
            }

            return Effect;
        }
    }
}
