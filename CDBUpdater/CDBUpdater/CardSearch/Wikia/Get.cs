using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;

using CDBUpdater.Helpers;
using CDBUpdater.Object;

namespace CDBUpdater.CardSearch.Wikia
{
    static class Get
    {
        public static string[] languages = new string[7] {
            "english", "french", "german", "italian", "spanish", "japanese", "japanese&#160;(base)" };

        public static Card Search(Data data, string cardUrl, Object.LoadSettings Load)
        {
            if (data == null || data.searchID.IsEmpty() || data.language.IsEmpty())
                return null;
            
            //string cardUrl = "http://yugioh.wikia.com/wiki/" + data.searchID;
            int langNum = data.language.Value();

            Card Card = new Card();
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
                    if (Load.Desc)
                    {
                        string[] Effect = Set.Effect(ref cardPage, data.language);
                        if (!Effect[0].IsEmpty())
                            Card.CardText = Effect[0];
                        if (!Effect[1].IsEmpty())
                            Card.PendulumEffect = Effect[1];
                    }
                }),
                Task.Run(() => {
                    if (Load.Pack)
                        Card.Packs = Set.Packs(ref cardPage, langNum, data.language);
                    if (Load.Ot)
                        Card.Appearance = Set.Appearance(ref cardPage, Card.Packs, data.language);
                })
            }.WaitAll();

            return Card;
        }

        public static int Value(this string language)
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
