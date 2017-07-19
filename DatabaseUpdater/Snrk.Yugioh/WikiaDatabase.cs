using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace Snrk.Yugioh
{
    public sealed class WikiaDatabase : IDisposable
    {
        /// <summary>
        /// Returns the current card.
        /// </summary>
        public DatabaseCard Card { get; private set; }
        /// <summary>
        /// The language of the downloaded card.
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
                this.m_OutputLanguage = Utility.HexToString(((int)value).ToString("x"));
            }
        }
        /// <summary>
        /// Specifys if card properties are loaded (id, attribute, pendulum scale, level, atk, def, race, types).
        /// </summary>
        public bool LoadProperties { get; set; }

        private string m_OutputLanguage;
        private HtmlDocument m_CardPage;
        private string[] m_Languages = new string[7] { "english", "french", "german", "italian", "spanish", "japanese", "japanese&#160;(base)" };

        /// <summary>
        /// Downloader for the Yu-Gi-Oh! Wikia database (http://yugioh.wikia.com/wiki/Yu-Gi-Oh!_Wikia).
        /// </summary>
        /// <param name="loadProperties">Loads card properties (id, attribute, pendulum scale, level, atk, def, race, types).</param>
        public WikiaDatabase(bool loadProperties = true)
        {
            this.Card = null;
            this.m_OutputLanguage = "en";
            this.LoadProperties = loadProperties;
        }

        /// <summary>
        /// Downloader for the Yu-Gi-Oh! Wikia database (http://yugioh.wikia.com/wiki/Yu-Gi-Oh!_Wikia).
        /// </summary>
        /// <param name="outputLanguage">The language of the downloaded cards.</param>
        /// <param name="loadProperties">Loads card properties (id, attribute, pendulum scale, level, atk, def, race, types).</param>
        public WikiaDatabase(DatabaseLanguage outputLanguage, bool loadProperties = true)
            : this(loadProperties)
        {
            this.m_OutputLanguage = Utility.HexToString(((int)outputLanguage).ToString("x"));
        }

        ~WikiaDatabase() { Dispose(); }
        /// <summary>
        /// Cleans everything up.
        /// </summary>
        public void Dispose()
        {
            this.Card?.Clear();
            m_Languages = null;
            m_CardPage = null;
        }
        
        /// <summary>
        /// Downloads card information from the Wikia web database.
        /// </summary>
        /// <param name="cardName">The name of the card.</param>
        /// <param name="cardId">The id of the card. Will be favoured over the card name. Useful when the name of the card has changed.</param>
        /// <returns>Returns the WikiaDatabase object.</returns>
        public WikiaDatabase Load(string cardName, string cardId = null)
        {
            this.Card?.Clear();
            this.Card = new DatabaseCard();
            if (!String.IsNullOrEmpty(cardId))
                ParseCard("http://yugioh.wikia.com/wiki/" + cardId.PadLeft(8, '0'));
            if (!String.IsNullOrEmpty(cardName) && (this.Card == null || String.IsNullOrEmpty(this.Card.Name)))
                ParseCard("http://yugioh.wikia.com/wiki/" + cardName);
            m_CardPage = null;

            return this;
        }
        
        private int GetLangValue(string language)
        {
            switch (language)
            {
                case "fr": return 1;
                case "de": return 2;
                case "it": return 3;
                case "es": return 4;
                case "ja": return 5;
                default: return 0;
            }
        }

        private void ParseCard(string cardUrl)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            try { m_CardPage = htmlWeb.Load(cardUrl); } catch { }
            htmlWeb = null;
            if (m_CardPage == null)
                return;

            this.Card.Language = this.m_OutputLanguage;
            int langValue = GetLangValue(this.m_OutputLanguage);

            new Task[] {
                this.LoadProperties ? ParsePropertiesAsync() : Task.CompletedTask,
                ParseCardNamesAsync(),
                ParseCardEffectsAsync(),
                ParsePacksAsync(langValue)
            }.WaitAll();

            if (this.Card.Name == null)
                this.Card.Appearance = 0;
        }

        #region ParseCardNames
        private Task ParseCardNamesAsync()
        {
            return Task.Run(() => {
                ParseCardNames();
            });
        }
        private void ParseCardNames()
        {
            if (m_CardPage == null)
                return;
            
            var nameNodes = m_CardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//td[@class='cardtablerowdata']//span[@lang='" + m_OutputLanguage + "']");
            if (nameNodes == null)
                nameNodes = m_CardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//th[@class='cardtablerowheader'][contains(., 'English')]//..//td[@class='cardtablerowdata']");

            if (nameNodes != null)
            {
                this.Card.Name = nameNodes.Count > 0 ? HttpUtility.HtmlDecode(nameNodes[nameNodes.Count > 1 ? 1 : 0].InnerText.Trim()
                    .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"')) : null;
                if (this.Card.Name != null)
                    this.Card.Name = System.Text.RegularExpressions.Regex.Replace(this.Card.Name, @"\p{Z}", " ");

                if (m_OutputLanguage == "ja")
                {
                    this.Card.NameKana = nameNodes.Count > 1 ? HttpUtility.HtmlDecode(nameNodes[0].InnerText.Trim()
                        .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"')) : this.Card.Name;
                    if (this.Card.NameKana != null)
                        this.Card.NameKana = System.Text.RegularExpressions.Regex.Replace(this.Card.NameKana, @"\p{Z}", " ");
                }
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

            var pendEffectNode = m_CardPage.DocumentNode.SelectNodes("//table[@class='collapsible " + (m_OutputLanguage == "en" ? "expanded" : "autocollapse") + " navbox-inner']" +
                "//dl//dd" + ((this.m_OutputLanguage == "en") ? "" : "//span[@lang='" + this.m_OutputLanguage + "']"));
            if (pendEffectNode != null)
            {
                for (int i = 0; i < pendEffectNode.Count && i < 2; ++i)
                {
                    string effect = HttpUtility.HtmlDecode(pendEffectNode[i].InnerHtml.Replace("<br>", Environment.NewLine)
                        .Trim().RemoveBetween('<', '>').Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"'));
                    if (effect != null)
                        effect = System.Text.RegularExpressions.Regex.Replace(effect, @"\p{Z}", " ");

                    if (i == 0)
                        this.Card.PendulumEffect = effect;
                    else
                        this.Card.CardText = effect;
                }
                pendEffectNode = null;
            }
            else
            {
                var effectNode = m_CardPage.DocumentNode.SelectSingleNode("//tr//td[@class='navbox-list']" + ((this.m_OutputLanguage == "en") ? "" : "//span[@lang='" + this.m_OutputLanguage + "']"));

                this.Card.CardText = HttpUtility.HtmlDecode(effectNode?.InnerHtml.Replace("<br>", Environment.NewLine).Trim().RemoveBetween('<', '>')
                    .Replace('‘', '\'').Replace('’', '\'').Replace('“', '"').Replace('”', '"'));
                if (this.Card.CardText != null)
                    this.Card.CardText = System.Text.RegularExpressions.Regex.Replace(this.Card.CardText, @"\p{Z}", " ");
                effectNode = null;
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
            //var nodeStats = m_CardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//a[@title='ATK']//..//..//td[@class='cardtablerowdata']//a");
            //string[] stats = nodeStats?.Select(n => n.InnerText).ToArray();

            var nodeTypes = m_CardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//a[@title='Type']//..//..//td[@class='cardtablerowdata']//a");
            string[] allTypes = nodeTypes?.Select(n => n.InnerText).ToArray();
            List<string> types = null;
            string race = null;

            if (allTypes != null && allTypes.Length > 0)
            {
                types = new List<string>();
                for (int i = 0; i < allTypes.Length; ++i)
                    if (i == 0)
                        race = allTypes[i];
                    else
                        types.Add(allTypes[i]);
                allTypes = null;
            }

            if (types == null)
            {
                var nodeType = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//th[@class='cardtablerowheader'][contains(., 'Type')]//..//td[@class='cardtablerowdata']//a");
                var nodeProperty = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//a[@title='Property']//..//..//td[@class='cardtablerowdata']//a");

                types = new List<string>();
                if (nodeType != null)
                {
                    types.Add(nodeType.InnerText);
                    if (types[0].Contains(" "))
                        types[0] = (" " + types[0]).GetBetween(" ", " ");
                }
                if (nodeProperty != null)
                    types.Add(nodeProperty.InnerText);

                nodeType = nodeProperty = null;
            }

            this.Card.Types = types;
            this.Card.Race = race;

            var nodeId = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//a[@title='Passcode']//..//..//td[@class='cardtablerowdata']//a");
            var nodeAttribute = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//a[@title='Attribute']//..//..//td[@class='cardtablerowdata']//a");
            var nodePendulumScale = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//a[@title='Pendulum Scale']//..//..//td[@class='cardtablerowdata']");
            var nodeLevel = m_CardPage.DocumentNode.SelectSingleNode("//table[@class='cardtable']//a[@title='Level']//..//..//td[@class='cardtablerowdata']//a");

            this.Card.Id = nodeId?.InnerText;
            this.Card.Attribute = nodeAttribute?.InnerText.ToTitleCase();
            this.Card.PendulumScale = nodePendulumScale?.InnerText.Trim();
            this.Card.Level = nodeLevel?.InnerText;

            var nodeStats = m_CardPage.DocumentNode.SelectNodes("//table[@class='cardtable']//a[@title='ATK']//..//..//td[@class='cardtablerowdata']//a");
            string[] stats = nodeStats?.Select(n => n.InnerText).ToArray();

            if (stats != null)
            {
                if (stats.Length > 0) this.Card.Atk = stats[0];
                if (stats.Length > 1) this.Card.Def = stats[1];
            }

            /*var properties = new CardProperties() {
                Id = nodeId?.InnerText,
                Attribute = nodeAttribute?.InnerText.ToTitleCase(),
                PendulumScale = nodePendulumScale?.InnerText.Trim(),
                Level = nodeLevel?.InnerText,
                Types = types,
                Race = race,
                Atk = stats != null && stats.Length > 0 ? stats[0] : null,
                Def = stats != null && stats.Length > 1 ? stats[1] : null
            };*/

            nodeId = nodeAttribute = nodePendulumScale = nodeLevel = null;
            stats = null;

            //return properties;
        }
        #endregion

        private Task ParsePacksAsync(int langValue)
        {
            return Task.Run(() => {
                this.Card.Packs = GetPacks(langValue);
                this.Card.Appearance = GetAppearance(this.Card.Packs);
            });
        }
        private List<DatabaseCard.Pack> GetPacks(int langValue)
        {
            List<DatabaseCard.Pack> Packs = new List<DatabaseCard.Pack>();

            if (m_CardPage == null)
                return Packs;

            var allTables = m_CardPage.DocumentNode.SelectNodes("//tr[@class='cardtablerow']//td[@class='cardtablespanrow']");

            if (allTables != null)
                foreach (var textTable in allTables)
                    if (textTable.SelectSingleNode(".//a[@title='Yu-Gi-Oh! Trading Card Game']") != null
                        || textTable.SelectSingleNode(".//a[@title='Yu-Gi-Oh! Official Card Game']") != null)
                    {
                        var setLangTable = textTable.SelectNodes(".//table[@class='collapsible autocollapse navbox-inner']");

                        if (setLangTable != null)
                            foreach (var setLang in setLangTable)
                            {
                                var langNode = setLang.SelectSingleNode(".//div[@style='font-size: 110%;']");
                                if (langNode == null)
                                    continue;
                                string lang = langNode.InnerText.ToLower();
                                if ((lang.Contains(m_Languages[langValue]) || (langValue == 0 ? lang.Contains(m_Languages[langValue] + "—worldwide") : false)))
                                {
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
                                                    DatabaseCard.Pack Set = new DatabaseCard.Pack();
                                                    for (int i = 0; i < detailedInfo.Count; ++i)
                                                    {
                                                        string content = detailedInfo[i].InnerText.Trim();

                                                        if (i == 0)
                                                            Set.ReleaseDate = content;
                                                        else if (i == 1)
                                                            Set.CardID = content;
                                                        else if (i == 2)
                                                        {
                                                            Set.EnglishPackName = content;
                                                            if (langValue == 0)
                                                                Set.PackName = content;
                                                        }
                                                        else if (i == 3 && langValue == 0)
                                                            Set.Rarity = content;
                                                        else if (i == 3 && langValue > 0)
                                                            Set.PackName = content;
                                                        else if (i == 4 && (lang.Contains(m_Languages[5]) || langValue > 0))
                                                            Set.Rarity = content;
                                                    }
                                                    Packs.Add(Set);
                                                }
                                                detailedInfo = null;
                                            }
                                            else isSet = true;
                                        }
                                    }
                                    setLine = null;
                                }
                                langNode = null;
                            }
                        setLangTable = null;
                    }
            allTables = null;

            return Packs;
        }

        private bool IsCardReleased(List<DatabaseCard.Pack> packs)
        {
            foreach (var pack in packs)
            {
                string releaseDate = '-' + pack.ReleaseDate;
                string[] tmp = new string[3] { "", "", "" };
                for (int i = 0, c = 0; i < releaseDate.Length - 1; ++i)
                    if (releaseDate[i] == '-' && c < tmp.Length)
                        tmp[++c - 1] += releaseDate[++i];
                    else if (c > 0)
                        tmp[c - 1] += releaseDate[i];
                if (tmp[0].Length < 1 || tmp[1].Length < 1 || tmp[2].Length < 1)
                    return false;
                int[] packDate = new int[3] { int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]) };
                int[] currDate = new int[] { DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day };
                if (packDate[0] - currDate[0] <= 0 && packDate[1] - currDate[1] <= 0 && packDate[2] - currDate[2] <= 0)
                    return true;
            }
            return false;
        }

        private int GetAppearance(List<DatabaseCard.Pack> packs)
        {
            bool isOcg = this.m_OutputLanguage == "ja";
            string otherLang = isOcg ? "en" : "ja";
            List<DatabaseCard.Pack> packsOther = GetPacks(GetLangValue(otherLang));

            bool isReleasedOther = IsCardReleased(packsOther);
            bool isReleasedThis = IsCardReleased(packs);
            
            return isReleasedThis || isReleasedOther ? ((isOcg ? isReleasedThis : isReleasedOther) ? 1 : 0) + ((isOcg ? isReleasedOther : isReleasedThis) ? 2 : 0) :
                ((isOcg ? packs.Count < 1 : packsOther.Count < 1) ? 1 : 0) + ((isOcg ? packsOther.Count < 1 : packs.Count < 1) ? 1 : 0);
        }
    }
}
