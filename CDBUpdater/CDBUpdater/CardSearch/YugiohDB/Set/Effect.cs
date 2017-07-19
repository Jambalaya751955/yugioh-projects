using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

using CDBUpdater.Helpers;

namespace CDBUpdater.CardSearch.YugiohDB
{
    partial class Set
    {
        public static string[] Effect(ref HtmlDocument cardPage)
        {
            string[] effect = new string[2];

            if (cardPage == null)
                return effect;

            List<string> content = new List<string>();
            List<string> remContent = new List<string>();

            HtmlNodeCollection remContent_n = cardPage.DocumentNode.SelectNodes("//div[@class='item_box_text']//div[@class='item_box_title']");
            HtmlNodeCollection content_n = cardPage.DocumentNode.SelectNodes("//div[@class='item_box_text']");
            
            if (remContent_n == null || content_n == null)
                return effect;

            foreach (HtmlNode n in remContent_n)
                remContent.Add(n.InnerText.HtmlDecode());
            foreach (HtmlNode n in content_n)
                content.Add(n.InnerHtml.HtmlDecode().Replace("<br>", "\n").RemoveBetween('<', '>'));

            if (content.Count >= 2)
                effect[1] = content[content.Count - 2].Remove(0, remContent[remContent.Count - 2].Length).Trim();
            if (content.Count >= 1)
                effect[0] = content[content.Count - 1].Remove(0, remContent[remContent.Count - 1].Length).Trim();

            return effect;
        }
    }
}
