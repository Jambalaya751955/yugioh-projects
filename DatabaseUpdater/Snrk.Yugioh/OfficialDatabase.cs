using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace Snrk.Yugioh
{
    public sealed class OfficialDatabase : IDisposable
    {
        /// <summary>
        /// Returns the current card.
        /// </summary>
        public DatabaseCard Card { get; private set; }
        /// <summary>
        /// The language of the name which passed into the Load method.
        /// </summary>
        public DatabaseLanguage InputLanguage
        {
            get
            {
                return (DatabaseLanguage)int.Parse(Utility.StringToHex(m_InputLanguage),
                    System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                m_InputLanguage = Utility.HexToString(((int)value).ToString("x"));
            }
        }
        /// <summary>
        /// The language of the downloaded cards.
        /// </summary>
        public DatabaseLanguage OutputLanguage
        {
            get
            {
                return (DatabaseLanguage)int.Parse(Utility.StringToHex(m_OutputLanguage),
                    System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                m_OutputLanguage = Utility.HexToString(((int)value).ToString("x"));
            }
        }
        /// <summary>
        /// Specifys if card properties are loaded (id, attribute, pendulum scale, level, atk, def, race, types).
        /// </summary>
        public bool LoadProperties { get; set; }

        private string m_InputLanguage;
        private string m_OutputLanguage;
        private HtmlDocument m_SearchPage;
        private HtmlDocument m_CardPage;

        /// <summary>
        /// Downloader for the Official Yu-Gi-Oh! database (http://www.db.yugioh-card.com/yugiohdb/).
        /// </summary>
        /// <param name="loadProperties">Loads card properties (id, attribute, pendulum scale, level, atk, def, race, types).</param>
        public OfficialDatabase(bool loadProperties = true)
        {
            this.Card = null;
            m_SearchPage = null;
            m_CardPage = null;
            m_InputLanguage = "en";
            m_OutputLanguage = "en";
            this.LoadProperties = loadProperties;
        }

        /// <summary>
        /// Downloader for the Official Yu-Gi-Oh! database (http://www.db.yugioh-card.com/yugiohdb/).
        /// </summary>
        /// <param name="outputLanguage">The language of the downloaded cards.</param>
        /// <param name="loadProperties">Loads card properties (id, attribute, pendulum scale, level, atk, def, race, types).</param>
        public OfficialDatabase(DatabaseLanguage outputLanguage, bool loadProperties = true)
            : this(loadProperties)
        {
            m_OutputLanguage = Utility.HexToString(((int)outputLanguage).ToString("x"));
        }

        /// <summary>
        /// Downloader for the Official Yu-Gi-Oh! database (http://www.db.yugioh-card.com/yugiohdb/).
        /// </summary>
        /// <param name="inputLanguage">The language of the name which passed into the Load method.</param>
        /// <param name="outputLanguage">The language of the downloaded cards.</param>
        /// <param name="loadProperties">Loads card properties (id, attribute, pendulum scale, level, atk, def, race, types).</param>
        public OfficialDatabase(DatabaseLanguage inputLanguage, DatabaseLanguage outputLanguage, bool loadProperties = true)
            : this(outputLanguage, loadProperties)
        {
            m_InputLanguage = Utility.HexToString(((int)inputLanguage).ToString("x"));
        }

        ~OfficialDatabase() { Dispose(); }
        /// <summary>
        /// Cleans everything up.
        /// </summary>
        public void Dispose()
        {
            this.Card.Clear();
            m_CardPage = m_SearchPage = null;
        }

        /// <summary>
        /// Downloads card information from the official database
        /// </summary>
        /// <param name="cardName">The name of the card.</param>
        /// <returns>Returns the OfficialDatabase object.</returns>
        public OfficialDatabase Load(string cardName)
        {
            m_SearchPage = null;
            m_CardPage = null;
            this.Card?.Clear();
            this.Card = new DatabaseCard();

            for (int i = 1; i <= 2; ++i)
            {
                string encodedName = Uri.EscapeDataString(cardName);
                DownloadSearchPage(encodedName, i);
                string cardUrl = GetCardUrl(encodedName);

                if (cardUrl != null)
                {
                    string url = string.Format("http://www.db.yugioh-card.com/yugiohdb/{0}&request_locale=", cardUrl);
                    ParseCard(url);
                    if (this.Card.Level == null)
                        ParseSpellType(cardName);
                    break;
                }
            }

            m_SearchPage = null;
            m_CardPage = null;

            return this;
        }

        private string GetCardUrl(string cardName)
        {
            if (m_SearchPage == null || String.IsNullOrEmpty(cardName))
                return null;

            HtmlNodeCollection rows = m_SearchPage.DocumentNode.SelectNodes("//dt[@class='box_card_name']//span[@class='card_status']");
            if (rows == null)
                return null;

            string cardUrl = null;
            cardName = HttpUtility.UrlDecode(cardName).ToLower();
            for (int i = 0; i < rows.Count && String.IsNullOrEmpty(cardUrl); ++i)
            {
                string rowName = Regex.Replace(HttpUtility.HtmlDecode(rows[i].InnerText), @"^\s*$\n", string.Empty, RegexOptions.Multiline).Trim().ToLower();
                if (cardName == rowName)
                {
                    HtmlNodeCollection urlValues = m_SearchPage.DocumentNode
                        .SelectNodes("//div[@class='list_style']//ul[@class='box_list']//li//input[@class='link_value']");

                    if (urlValues != null)
                    {
                        for (int j = 1, jc = urlValues.Count + 1; j < jc; ++j)
                            if (j == i + 1)
                            {
                                cardUrl = urlValues[j - 1].Attributes["value"].Value;
                                break;
                            }
                        urlValues = null;
                    }
                }
            }
            
            rows = null;

            return cardUrl;
        }

        private void DownloadSearchPage(string cardName, int page = 1)
        {
            if (String.IsNullOrEmpty(cardName))
                return;
            
            string searchUrl = string.Format("card_search.action?ope=1&sess=1&keyword={0}", cardName);
            string url = string.Format("http://www.db.yugioh-card.com/yugiohdb/{0}&request_locale={1}&page={2}",
                searchUrl, m_InputLanguage, page > 0 ? page.ToString() : "1");

            HtmlWeb htmlWeb = new HtmlWeb();
            try { m_SearchPage = htmlWeb.Load(url); } catch { }
            htmlWeb = null;
        }

        private void ParseCard(string url)
        {
            if (String.IsNullOrEmpty(url))
                return;

            HtmlWeb htmlWeb = new HtmlWeb();
            try { m_CardPage = htmlWeb.Load(url + m_OutputLanguage); } catch { }
            htmlWeb = null;

            if (m_CardPage == null)
                return;
            
            new Task[] {
                ParseCardNameAsync(),
                ParseCardEffectsAsync(),
                ParsePacksAsync(),
                this.LoadProperties ? ParsePropertiesAsync() : Task.CompletedTask
            }.WaitAll();

            this.Card.Language = m_OutputLanguage;
        }

        #region ParseCardName
        private Task ParseCardNameAsync()
        {
            return Task.Run(() => {
                ParseCardName();
            });
        }
        private void ParseCardName()
        {
            if (m_CardPage == null)
                return;

            if (m_OutputLanguage == "ja")
            {
                HtmlNode nameNode = m_CardPage.DocumentNode.SelectSingleNode("//header[@id='broad_title']//h1");
                if (nameNode != null)
                {
                    this.Card.Name = HttpUtility.HtmlDecode(nameNode.InnerHtml?.Replace(Environment.NewLine, String.Empty)
                        .GetBetween("</span>", "<span>").Trim()
                        .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"'));
                    this.Card.NameKana = HttpUtility.HtmlDecode(nameNode.SelectSingleNode("//span[@class='ruby']")?.InnerText
                        .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"'));
                }
                else
                    return;
                nameNode = null;
            }
            else
            {
                var headerNode = m_CardPage.DocumentNode.SelectSingleNode("//header[@id='broad_title']//h1[text()]");
                this.Card.Name = headerNode?.InnerText.Trim()
                    .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"');
            }
        }
        #endregion

        #region ParseCardEffects
        private Task ParseCardEffectsAsync()
        {
            return Task.Run(() => {
                ParseCardEffects();
            });
        }
        private void ParseCardEffects()
        {
            if (m_CardPage == null)
                return;

            var effectNodes = m_CardPage.DocumentNode.SelectNodes("//div[@class='item_box_text']/text()");
            string[] effects = new string[2] { null, null };
            for (int i = 0, c = 0; i < effectNodes.Count && c < effects.Length; ++i)
            {
                string innerText = HttpUtility.HtmlDecode(effectNodes[i].InnerText.Trim()
                    .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"'));
                if (innerText.Length < 1 && effects[c] != null)
                    ++c;
                else
                    effects[c] += (effects[c] != null && effects[c].Length > 0 ? Environment.NewLine : "") + innerText;
            }

            this.Card.CardText = effects[0];
            this.Card.PendulumEffect = effects[1];

            effectNodes = null;
            effects = null;
        }
        #endregion

        #region ParsePacks
        private Task ParsePacksAsync()
        {
            return Task.Run(() => {
                ParsePacks();
            });
        }
        private void ParsePacks()
        {
            HtmlNodeCollection parent = m_CardPage.DocumentNode.SelectNodes("//tr[@class='row']");
            if (parent == null)
                return;

            foreach (HtmlNode childs in parent)
            {
                if (childs.InnerText.Contains("[") || childs.InnerText.Contains("]"))
                    continue;

                DatabaseCard.Pack Set = new DatabaseCard.Pack();

                HtmlNode rd = childs.SelectSingleNode(".//td[@class='t_center']");
                if (rd != null)
                {
                    Set.ReleaseDate = HttpUtility.HtmlDecode(rd.InnerText);
                    rd = null;
                }

                HtmlNode pn = childs.SelectSingleNode(".//td//b");
                if (pn != null)
                {
                    string packName = pn.InnerText.ToTitleCase();
                    Set.PackName = HttpUtility.HtmlDecode(packName);
                    if (m_OutputLanguage == "en")
                    {
                        Set.EnglishPackName = packName;
                    }
                    pn = null;
                }

                bool set = false;
                HtmlNodeCollection cid = childs.SelectNodes(".//td");
                if (cid != null)
                {
                    foreach (HtmlNode child in cid)
                        if (set)
                        {
                            Set.CardID = HttpUtility.HtmlDecode(child.InnerText);
                            break;
                        }
                        else set = true;
                    cid = null;
                }

                HtmlNode ra = childs.SelectSingleNode(".//td//img");
                if (ra != null)
                {
                    Set.Rarity = HttpUtility.HtmlDecode(ra.Attributes["alt"].Value);
                    ra = null;
                }
                else
                    switch (m_OutputLanguage)
                    {
                        case "en": case "de": Set.Rarity = "Common"; break;
                        case "it": Set.Rarity = "Comune"; break;
                        case "es": Set.Rarity = "Común"; break;
                        case "fr": Set.Rarity = "Commune"; break;
                        case "ja": Set.Rarity = "共通"; break;

                    }

                this.Card.Packs.Add(Set);
            }
        }
        #endregion

        #region ParseProperties
        private Task ParsePropertiesAsync()
        {
            return Task.Run(() => {
                ParseProperties();
            });
        }
        private void ParseProperties()
        {
            HtmlNodeCollection values = m_CardPage.DocumentNode.SelectNodes("//span[@class='item_box_value']");

            if (values != null)
            {
                for (int i = 0; i < values.Count; ++i)
                {
                    string value = HttpUtility.HtmlDecode(values[i].InnerText).Trim();
                    switch (i)
                    {
                        case 0: this.Card.Attribute = value.ToTitleCase(); break;
                        case 1: this.Card.Level = value; break;
                        case 2: this.Card.Atk = value; break;
                        case 3: this.Card.Def = value; break;
                    }
                }
                values = null;
            }
            
            var nodes = m_CardPage.DocumentNode.SelectNodes("//div[@class='item_box t_center']/text()");
            string[] cells = new string[3];
            for (int i = 0, c = 0; i < nodes.Count && c < cells.Length; ++i)
            {
                string innerText = nodes[i].InnerText.Trim();
                if (innerText.Length < 1 && cells[c] != null)
                    ++c;
                else
                    cells[c] += innerText;
            }

            for (int i = 0; i < 2 && cells[2] == null; ++i)
            {
                cells[2] = cells[1];
                cells[1] = cells[0];
                cells[0] = null;
            }

            this.Card.PendulumScale = cells[0];
            this.Card.Race = cells[1];
            string[] types = cells[2]?.Split('/', '／');
            this.Card.Types = types == null ? new List<string>() : new List<string>(types);
            types = null;

            nodes = null;
            cells = null;
        }
        #endregion

        private void ParseSpellType(string cardName)
        {
            if (m_SearchPage == null)
                return;

            this.Card.Types = new List<string>();

            var _i = m_SearchPage.DocumentNode.SelectSingleNode("//dt[@class='box_card_name']//span[@class='card_status']" +
                "//strong[translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + cardName.ToLower() + "']" +
                "/ancestor::dl[1]//span[@class='box_card_attribute']//img");
            if (_i != null)
            {
                string _attr = GetSpellType(_i.Attributes["src"].Value, m_OutputLanguage);
                if (_attr != "通常" && _attr != "Normal" && _attr != "Normale")
                    this.Card.Types.Add(_attr);
            }
            
            var _j = m_SearchPage.DocumentNode.SelectSingleNode("//dt[@class='box_card_name']//span[@class='card_status']" +
                "//strong[translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + cardName.ToLower() + "']" +
                "/ancestor::dl[1]//span[@class='box_card_effect']//img");
            if (_j != null)
                this.Card.Types.Add(GetSpellType(_j.Attributes["src"].Value, m_OutputLanguage));
        }

        private string GetSpellType(string type, string language)
        {
            if (type.Contains("spell"))
                switch (language)
                {
                    case "ja": return "魔法";
                    case "en": return "Spell";
                    case "de": return "Zauber";
                    case "fr": return "Magie";
                    case "it": return "Magia";
                    case "es": return "Mágica";
                }
            else if (type.Contains("trap"))
                switch (language)
                {
                    case "ja": return "罠";
                    case "en": return "Trap";
                    case "de": return "Falle";
                    case "fr": return "Piège";
                    case "it": return "Trappola";
                    case "es": return "Trampa";
                }
            else if (type.Contains("equip"))
                switch (language)
                {
                    case "ja": return "装備";
                    case "en": return "Equip";
                    case "de": return "Ausrüstung";
                    case "fr": return "Équiper";
                    case "it": return "Equipaggiamento";
                    case "es": return "Equipar";
                }
            else if (type.Contains("field"))
                switch (language)
                {
                    case "ja": return "フィールド";
                    case "en": return "Field";
                    case "de": return "Feld";
                    case "fr": return "Champ";
                    case "it": return "Terreno";
                    case "es": return "Campo";
                }
            else if (type.Contains("quickplay"))
                switch (language)
                {
                    case "ja": return "速攻";
                    case "en": return "Quick-Play";
                    case "de": return "Schnell";
                    case "fr": return "Jeu-Rapide";
                    case "it": return "Rapida";
                    case "es": return "Juego Rápido";
                }
            else if (type.Contains("ritual"))
                switch (language)
                {
                    case "ja": return "儀式";
                    case "en": case "de": case "es": return "Ritual";
                    case "fr": return "Rituel";
                    case "it": return "Rituale";
                }
            else if (type.Contains("continuous"))
                switch (language)
                {
                    case "ja": return "永続";
                    case "en": return "Continuous";
                    case "de": return "Permanent";
                    case "fr": return "Continue";
                    case "it": case "es": return "Continua";
                }
            else if (type.Contains("counter"))
                switch (language)
                {
                    case "ja": return "カウンター";
                    case "en": return "Counter";
                    case "de": return "Konter";
                    case "fr": return "Contre";
                    case "it": return "Contro";
                    case "es": return "de Contraefecto";
                }
            else
                switch (language)
                {
                    case "ja": return "通常";
                    case "en": case "de": case "fr": case "es": return "Normal";
                    case "it": return "Normale";
                }
            return type;
        }
    }
}
