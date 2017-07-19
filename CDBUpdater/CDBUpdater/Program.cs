using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Data.SQLite;

using CDBUpdater.Helpers;
using CDBUpdater.Object;
using CDBUpdater.SQLite;
using CDBUpdater.SQLite.CDB;

using System.Web;

namespace CDBUpdater
{
    static class Program
    {
        static void Main()
        {
            Input Input = Menu.GetInput();
            //Input.Load.Pack = false;
            FormatString = System.IO.File.ReadAllText(Input.FormatDirectory + "/desc.format.txt");
            
            List<int> loadIds = new List<int>();
            if (!Input.Load.All)
            {
                int years = (int)(Input.Months / 12);
                int months = Input.Months - years * 12;
                List<int> currDate = new List<int>() { DateTime.Now.Year - years, DateTime.Now.Month - months, DateTime.Now.Day};
                
                foreach (string[] row in Input.Packs)
                {
                    if (String.IsNullOrWhiteSpace(row[1]))
                        continue;
                    string date = '-' + row[1];
                    
                    List<string> dateListTemp = new List<string>();

                    if (date != null)
                        for (int i = 0; i < date.Length; ++i)
                            if (date[i] == '-')
                            {
                                dateListTemp.Add("");
                                dateListTemp[dateListTemp.Count - 1] += date[++i];
                            }
                            else
                                dateListTemp[dateListTemp.Count - 1] += date[i];

                    List<int> packDate = new List<int>();
                    for (int i = 0, element = 0; i < dateListTemp.Count; ++i)
                        if (int.TryParse(dateListTemp[i], out element))
                            packDate.Add(element);
                    if (packDate.Count != 3)
                        continue;
                    int id = 0;
                    if (packDate[0] > currDate[0] ? true : (packDate[0] == currDate[0] ? (packDate[1] > currDate[1] ? true : packDate[2] >= currDate[2]) : false) && int.TryParse(row[0], out id))
                        loadIds.Add(id);
                }
            }

            if ((Input.Load.Wikia || Input.Load.YGODB) && (Input.Load.Name || Input.Load.Desc || Input.Load.Ot))
            {
                /// get all cards from entered Cdb and
                /// create Input objects for yugiohdb and wikia search
                List<CDBName> cdbData = Load.GetAllCards(Input.CdbPath);
                List<Data> cardData = new List<Data>();

                foreach (CDBName cdbRow in cdbData)
                    if (cdbRow.ID != null && cdbRow.ID.Count > 0)
                    {
                        List<string> ids = cdbRow.ID.Select(id => id.ToString()).ToList();
                        if (ids.Count < 1 || loadIds.Count > 0 ? !cdbRow.ID.Any(cId => loadIds.Contains(cId)) : false)
                            continue;
                        string searchID = String.Concat(Enumerable.Repeat("0", ids[0].Length <= 8 ? 8 - ids[0].Length : 0)) + ids[0];
                        Data searchData = new Data(ids, searchID, Input.Language, cdbRow.Name);
                        cardData.Add(searchData);
                    }

                /// save paths in a dictionary for later access
                Dictionary<string, string> dir = new Dictionary<string, string>() {
                    { "root", Input.CdbPath + @"\"}, { "save", Input.SavingDirectory + @"\" },
                    { "cards", Input.SavingDirectory + @"\cards_" + Input.Language + ".cdb" },
                    { "pack", Input.SavingDirectory + @"\pack_" + Input.Language + ".cdb" }
                };

                /// create cdbs
                Create.CDB(dir["cards"], Cards.GetCreateSQL());
                if (Input.Load.Pack)
                    Create.CDB(dir["pack"], Pack.GetCreateSQL());

                /// download cards
                double interval = 3.0;
                Console.WriteLine(GetConsoleOut(interval, -1, cardData.Count));

                /// Load already loaded information
                /*List<string[]> partialCdb = new List<string[]>();
                SQLiteConnection connection = new SQLiteConnection("Data Source=" + dir["cards"]);
                try
                {
                    connection.Open();
                    SQLiteCommand textcommand = new SQLiteCommand("SELECT id FROM texts", connection);
                    partialCdb = Commands.ExecuteStringCommand(textcommand, 19);
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }*/

                File.AppendAllText(dir["save"] + "notloaded.txt", string.Format("{0}-- {1} --{0}", Environment.NewLine,
                    string.Format("{0}-{1}-{2} / {3}:{4}:{5}", DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                        DateTime.UtcNow.Hour.ToString("00"), DateTime.UtcNow.Minute.ToString("00"), DateTime.UtcNow.Second.ToString("00"))));

                Task[] tasks = new Task[50];
                for (int index = 0; index < cardData.Count; ++index)
                {
                    if (WriteCDB[0] != null)
                        WriteCDB[0].Wait();
                    if (WriteCDB[1] != null)
                        WriteCDB[1].Wait();

                    for (int i = 0; i < tasks.Length; ++i)
                    {
                        if (tasks[i] == null || tasks[i].IsCompleted)
                        {
                            tasks[i] = Task.Run(() =>
                            {
                                int localIndex = index;
                                double searchTime = SearchCard(cardData[localIndex], dir, Input.Load);
                                interval = (interval + searchTime) / 3.0;
                                Console.SetCursorPosition(0, 2);
                                Console.WriteLine(GetConsoleOut(interval, localIndex, cardData.Count));
                                Console.SetCursorPosition(0, 2);
                            });
                            break;
                        }
                        if (i == tasks.Length - 1)
                        {
                            i = 0;
                            Thread.Sleep(200);
                        }
                    }

                    Thread.Sleep((int)(interval * 1000.0 + (interval < 1.25 ? (1.25 - interval) * 1000.0 : 0.0)));
                }
                tasks.WaitAll();
                Thread.Sleep((int)(interval * 2000.0 + (interval < 1.25 ? (1.25 - interval) * 2000.0 : 0.0)));
            }

            Notification.FlashWindow(Process.GetCurrentProcess().MainWindowHandle);
            Console.WriteLine(" The download has finished.");
            Console.ReadLine();
            return;
        }

        public static string FormatString = "{CardText}";
        private static Task[] WriteCDB = new Task[2];

        private static double SearchCard(Data data, Dictionary<string, string> dir, Object.LoadSettings Load)
        {
            Data dataCopy = data;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Card CardMerged = new Card();
            CardMerged.IDs = dataCopy.IDs;
            CardMerged.Language = dataCopy.language.ToLower();

            Card CardWikia = null;
            if (Load.Wikia)
            {
                Card CardWikiaName = CardSearch.Wikia.Get.Search(dataCopy, "http://yugioh.wikia.com/wiki/" + data.cdbName, Load);
                Card CardWikiaId = CardSearch.Wikia.Get.Search(dataCopy, "http://yugioh.wikia.com/wiki/" + data.searchID, Load);
                CardWikia = CardWikiaId != null && !CardWikiaId.Name.IsEmpty() ? CardWikiaId : CardWikiaName;
                if (CardWikia != null && !CardWikia.Name.IsEmpty())
                    data.encodedName = CardWikia.Name.UriEscape();
                if (dataCopy.encodedName.IsEmpty())
                {
                    string currentDate = string.Format("{0}:{1}:{2}", DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"));
                    File.AppendAllText(dir["save"] + "notloaded.txt", string.Format("{0}[{1}]({2}) {3}", Environment.NewLine, currentDate, dataCopy.searchID, dataCopy.cdbName));
                    return sw.Elapsed.ToDouble();
                }
            }

            Card CardYGODB = null;
            if (Load.YGODB)
            {
                HtmlDocument searchPage = null;
                string cardUrl = null;
                for (int i = 1; cardUrl.IsEmpty() && i <= 2; ++i)
                {
                    searchPage = CardSearch.YugiohDB.Get.SearchPageDocument(dataCopy.encodedName, dataCopy.language, i);
                    cardUrl = CardSearch.YugiohDB.Get.CardUrl(ref searchPage, dataCopy.encodedName);
                }

                CardYGODB = null;
                if (cardUrl != null)
                {
                    dataCopy.ygodbUrl = string.Format("http://www.db.yugioh-card.com/yugiohdb/{0}&request_locale=", cardUrl);
                    CardYGODB = CardSearch.YugiohDB.Get.CardInfo(searchPage, dataCopy, Load);
                }
            }

            if (Load.Ot) CardMerged.Appearance = CardWikia != null ? CardWikia.Appearance : 0;
            else CardMerged.Appearance = 0;

            if (Load.Desc) CardMerged.CardText = CardYGODB != null && !CardYGODB.CardText.IsEmpty() ? CardYGODB.CardText : (CardWikia != null ? CardWikia.CardText : null);
            else CardMerged.CardText = null;

            if (Load.Name) CardMerged.Name = CardYGODB != null && !CardYGODB.Name.IsEmpty() ? CardYGODB.Name : (CardWikia != null ? CardWikia.Name : null);
            else CardMerged.Name = null;

            if (Load.Desc) CardMerged.PendulumEffect = CardYGODB != null && !CardYGODB.PendulumEffect.IsEmpty() ? CardYGODB.PendulumEffect : (CardWikia != null ? CardWikia.PendulumEffect : null);
            else CardMerged.PendulumEffect = null;

            if (Load.Pack)
                if (CardYGODB != null && CardYGODB.Packs != null && CardYGODB.Packs.Count > 0)
                    CardMerged.Packs = CardYGODB.Packs;
                else if (CardWikia != null && CardWikia.Packs != null)
                {
                    for (int i = 0; i < CardWikia.Packs.Count; ++i)
                        if (CardWikia.Packs[i] == null || CardWikia.Packs[i].PackName.IsEmpty() || CardWikia.Packs[i].CardID.IsEmpty())
                            CardWikia.Packs.Remove(CardWikia.Packs[i--]);
                    CardMerged.Packs = CardWikia.Packs;
                }

            if (Load.Pack && CardMerged.Packs != null && CardMerged.Packs.Count > 0)
                WriteCDB[1] = Task.Run(() =>
                    Commands.ExecuteMethod<bool, List<Card.Pack>, List<string>>(dir["pack"], Pack.InsertPack, CardMerged.Packs, CardMerged.IDs));

            if (CardMerged == null || Load.Name ? CardMerged.Name.IsEmpty() : false || Load.Desc ? CardMerged.CardText.IsEmpty() : false)
            {
                string currentDate = string.Format("{0}:{1}:{2}", DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"));
                File.AppendAllText(dir["save"] + "notloaded.txt", string.Format("{0}[{1}]({2}) {3}", Environment.NewLine, currentDate, dataCopy.searchID, dataCopy.cdbName));
            }
            else
                WriteCDB[0] = Task.Run(() =>
                    Commands.ExecuteMethod<bool, Card, Data>(dir["cards"], Cards.InsertCard, CardMerged, dataCopy));

            return sw.Elapsed.ToDouble();
        }
        
        private static double ToDouble(this TimeSpan elapsed)
        {
            double elapsedTime = 0.0;
            if (double.TryParse(elapsed.ToString().Replace('.', ','), out elapsedTime))
                return elapsedTime;
            else
                return 3.0;
        }
        
        public static string GetConsoleOut(double searchInterval, int index, int totalCount)
        {
            string percentage = (Convert.ToDouble(index + 1) / totalCount * 100).ToString().Replace(',', '.');
            percentage = percentage.Substring(0, percentage.Length >= 4 ? 4 : percentage.Length);
            percentage = String.Concat(Enumerable.Repeat(" ", percentage.Length <= 3 ? 3 - percentage.Length : 0)) + percentage;

            double remaining = searchInterval * (totalCount - index + 1);
            int hours = (int)(remaining / 3600);
            int minutes = ((int)(remaining / 60)) % 60;
            int seconds = (int)remaining % 60;
            string remainingTime = string.Format("{0}:{1}:{2}",
                hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00"));

            return string.Format("  Downloaded: {0}% [{1}/{2}]     \n   Remaining: {3}{4}",
                percentage, index + 1, totalCount, remainingTime,
                String.Concat(Enumerable.Repeat(" ", 120)));
        }
    }
}
