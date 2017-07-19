using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

using CDBUpdater.Object;
using CDBUpdater.Helpers;

namespace CDBUpdater.CardSearch.YugiohDB
{
    partial class Set
    {
        public static string Name(ref HtmlDocument cardPage, string language)
        {
            if (cardPage == null || language.IsEmpty())
                return null;
            
            if (language.ToLower() == "ja")
            {
                HtmlNode nameNode = cardPage.DocumentNode.SelectSingleNode("//header[@id='broad_title']//h1");
                if (nameNode != null)
                    return nameNode.InnerHtml.HtmlDecode().Replace(Environment.NewLine, String.Empty).GetBetween("</span>", "<span>").Trim();
                else
                    return null;
            }
            else
            {
                List<string> content = new List<string>();
                List<string> remContent = new List<string>();

                HtmlNode content_n = cardPage.DocumentNode.SelectSingleNode("//header[@id='broad_title']//h1");
                HtmlNode remContent_n = cardPage.DocumentNode.SelectSingleNode("//header[@id='broad_title']//h1//span");

                if (content_n != null)
                    content.Add(content_n.InnerText.HtmlDecode().Trim());

                remContent.Add((remContent_n == null) ? null : remContent_n.InnerText.HtmlDecode().Trim());

                if (content.Count >= 1 && remContent[remContent.Count - 1] == null)
                    return content[content.Count - 1].Trim().HtmlDecode();
                else if (remContent.Count >= 1 && content.Count >= 1)
                {
                    int lcr = remContent[remContent.Count - 1].Length;
                    return content[content.Count - 1]
                        .Remove(content[content.Count - 1].Length - lcr, lcr)
                        .Trim().HtmlDecode();
                }
            }

            return null;
        }
    }
}
