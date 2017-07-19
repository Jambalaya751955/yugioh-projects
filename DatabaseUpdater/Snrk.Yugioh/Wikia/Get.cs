using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace DatabaseDownloader.Wikia_
{
    static class Get
    {
        public static string[] languages = new string[7] {
            "english", "french", "german", "italian", "spanish", "japanese", "japanese&#160;(base)" };

        public static DatabaseCard Search(string cardUrl, string language)
        {
            if (String.IsNullOrEmpty(language))
                return null;

            //string cardUrl = "http://yugioh.wikia.com/wiki/" + data.searchID;
            int langNum = language.Value();

            DatabaseCard Card = new DatabaseCard();
            HtmlDocument cardPage = null;
            try { cardPage = new HtmlWeb().Load(cardUrl); }
            catch { return null; }
            if (cardPage == null)
                return null;

            System.Threading.Thread.Sleep(15);

            //Card.Name = Set.Name(ref cardPage, langNum);
            //if (!Card.Name.IsEmpty())
            //    data.encodedName = Card.Name.UriEscape();

            System.Threading.Thread.Sleep(15);

            new Task[] {
                Task.Run(() => {
                    Card.Name = Set.Name(ref cardPage, langNum);
                }),
                Task.Run(() => {
                    string[] Effect = Set.Effect(ref cardPage, language);
                    if (!String.IsNullOrEmpty(Effect[0]))
                        Card.CardText = Effect[0];
                    if (!String.IsNullOrEmpty(Effect[1]))
                        Card.PendulumEffect = Effect[1];
                }),
                Task.Run(() => {
                    Card.Appearance = Set.Appearance(ref cardPage);
                }),
                Task.Run(() => {
                    Card.Packs = Set.Packs(ref cardPage, langNum, language);
                })
            }.WaitAll();

            return Card;
        }

        private static int Value(this string language)
        {
            switch (language.ToLower())
            {
                case "fr": return 1;
                case "de": return 2;
                case "it": return 3;
                case "es": return 4;
                case "ja": return 5;
                default: return 0;
            }
        }
    }
}
