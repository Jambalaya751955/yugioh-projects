using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;

using CDBUpdater.Object;
using CDBUpdater.Helpers;

namespace CDBUpdater.CardSearch.YugiohDB
{
    class Get
    {
        public static Card CardInfo(HtmlDocument searchPage, Data data, Object.LoadSettings Load)
        {
            if (data == null || data.ygodbUrl.IsEmpty() || data.language.IsEmpty())
                return null;

            Card Card = new Card();
            HtmlDocument cardPage = null;
            try { cardPage = new HtmlWeb().Load(data.ygodbUrl + data.language); }
            catch { return null; }
            if (cardPage == null)
                return null;

            System.Threading.Thread.Sleep(15);

            new Task[] {
                Task.Run(() => {
                    if (Load.Name)
                        Card.Name = Set.Name(ref cardPage, data.language);
                }),
                Task.Run(() => {
                    if (Load.Desc)
                    {
                        string[] Effect = Set.Effect(ref cardPage);
                        if (!Effect[0].IsEmpty())
                            Card.CardText = Effect[0];
                        if (!Effect[1].IsEmpty())
                            Card.PendulumEffect = Effect[1];
                    }
                }),
                Task.Run(() => {
                    if (Load.Pack)
                        Card.Packs = Set.Packs(ref cardPage, data.language);
                })
            }.WaitAll();

            return Card;
        }

        public static string CardUrl(ref HtmlDocument searchPage, string cardName)
        {
            if (searchPage == null || cardName.IsEmpty())
                return null;

            HtmlNodeCollection rows = searchPage.DocumentNode.SelectNodes("//dt[@class='box_card_name']//span[@class='card_status']");
            if (rows == null)
                return null;

            string cardUrl = null;
            try
            {
                /*
                if (rows.Count == 1)
                    return searchPage.DocumentNode.SelectSingleNode(
                        "//div[@class='list_style']//ul[@class='box_list']//li//input[@class='link_value']")
                        .Attributes["value"].Value;
                */

                int line = 1;
                foreach (HtmlNode row in rows)
                {
                    string name = Regex.Replace(row.InnerText.HtmlDecode(), @"^\s*$\n", string.Empty, RegexOptions.Multiline).Trim();
                    if (cardName.UrlDecode().ToLower() == name.ToLower())
                    {
                        HtmlNodeCollection urlValues = searchPage.DocumentNode.SelectNodes(
                            "//div[@class='list_style']//ul[@class='box_list']//li//input[@class='link_value']");

                        if (urlValues != null)
                            for (int i = 1; i < urlValues.Count + 1; ++i)
                                if (i == line)
                                {
                                    cardUrl = urlValues[i - 1].Attributes["value"].Value;
                                    break;
                                }
                    }

                    if (!cardUrl.IsEmpty())
                        break;
                    ++line;
                }
            }
            catch
            {
            }

            return cardUrl;
        }

        public static HtmlDocument SearchPageDocument(string cardName, string language, int page = 1)
        {
            if (cardName.IsEmpty() || language.IsEmpty())
                return null;

            HtmlDocument siteDoc = new HtmlDocument();
            string searchUrl = string.Format("card_search.action?ope=1&sess=1&keyword={0}", cardName);
            string url = string.Format("http://www.db.yugioh-card.com/yugiohdb/{0}&request_locale={1}&page={2}",
                searchUrl, language, page > 0 ? page.ToString() : "1");

            try { siteDoc = new HtmlWeb().Load(url); } catch { }
            return siteDoc != null ? siteDoc : null;
        }
    }
}
