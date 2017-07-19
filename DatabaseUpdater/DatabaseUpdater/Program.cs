using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Snrk.Github;
using Snrk.SQLite;
using Snrk.Text;
using Snrk.Yugioh;

namespace DatabaseUpdater
{
    static class Program
    {
        static void Main()
        {
            //string time = Utility.GetZuluString();
            //if (true) ;

            /**/
            var repositoryInfo = new GithubRepositoryInfo("Snrk0", "live2016", "master", Information.LiveDirectory);
            var live2016 = new GithubRepository(repositoryInfo);
            live2016.Clone(true, true);
            
            var databaseFileNames = new string[0];
            string downloadDirectory = Path.Combine(Information.LiveDirectory, "cdb");
            using (GithubDownloader githubDownloader = new GithubDownloader("https://github.com/Ygoproco/Live2016"))
                databaseFileNames = githubDownloader.DownloadFiles(downloadDirectory, n => n.StartsWith("prerelease") && n.EndsWith(".cdb")).ToArray();

            var changelog = new Dictionary<string, Changelog>();

            using (var wikiaDatabase = new WikiaDatabase(DatabaseLanguage.English, false))
            using (var databaseTranslator = new DatabaseTranslator())
            {
                foreach (var databaseFileName in databaseFileNames)
                {
                    changelog.Add(databaseFileName, new Changelog(databaseFileName));

                    string filePath = Path.Combine(live2016.Info.Directory, databaseFileName);
                    SQLite.CreateDatabase(filePath);

                    using (var downloadConnection = new SQLiteConnection(Path.Combine(downloadDirectory, databaseFileName)))
                    using (var updateConnection = new SQLiteConnection(filePath))
                    {
                        downloadConnection.Open();
                        updateConnection.Open();

                        updateConnection.CreateTable("texts", Information.TextsColumns);
                        updateConnection.CreateTable("datas", Information.DatasColumns);

                        foreach (var rowTexts in downloadConnection.ReadRows("texts", new[] { "id", "name", "desc", "str1", "str2", "str3",
                            "str4", "str5", "str6", "str7", "str8", "str9", "str10", "str11", "str12", "str13", "str14", "str15", "str16" }))
                        {
                            var rowDatas = downloadConnection.ReadRows("WHERE id = " + rowTexts["id"], "datas", new[] { "id", "ot", "alias",
                                "setcode", "type", "atk", "def", "level", "race", "attribute", "category" }).ToArray().FirstOrDefault();

                            if (rowDatas == null || rowDatas.Count == 0)
                                continue;

                            var card = wikiaDatabase.Load(rowTexts["name"], rowTexts["id"]).Card;

                            var effect = new DatabaseCard.Effect(card.CardText, card.PendulumEffect);
                            string updatedDescription = databaseTranslator.ToDescriptionString(effect, "en");
                            if (!string.IsNullOrEmpty(effect.PendulumEffect) || !string.IsNullOrEmpty(effect.DeckMasterEffect))
                            {
                                int index = rowTexts["desc"].IndexOfAny(new[] { '\r', '\n' });
                                if (index > -1)
                                    updatedDescription = rowTexts["desc"].Substring(0, index) + "\r\n" + updatedDescription;
                            }

                            string[] textsValues = new string[19]
                            {
                                !String.IsNullOrEmpty(card.Id) ? card.Id : rowTexts["id"],
                                !String.IsNullOrEmpty(card.Name) ? card.Name : rowTexts["name"],
                                !String.IsNullOrEmpty(updatedDescription) ? updatedDescription : rowTexts["desc"],
                                rowTexts["str1"],
                                rowTexts["str2"],
                                rowTexts["str3"],
                                rowTexts["str4"],
                                rowTexts["str5"],
                                rowTexts["str6"],
                                rowTexts["str7"],
                                rowTexts["str8"],
                                rowTexts["str9"],
                                rowTexts["str10"],
                                rowTexts["str11"],
                                rowTexts["str12"],
                                rowTexts["str13"],
                                rowTexts["str14"],
                                rowTexts["str15"],
                                rowTexts["str16"]
                            };
                            string[] datasValues = new string[11]
                            {
                                !String.IsNullOrEmpty(card.Id) ? card.Id : rowDatas["id"],
                                card.Appearance > 0 ? card.Appearance.ToString() : rowDatas["ot"],
                                rowDatas["alias"],
                                rowDatas["setcode"],
                                rowDatas["type"],
                                rowDatas["atk"],
                                rowDatas["def"],
                                rowDatas["level"],
                                rowDatas["race"],
                                rowDatas["attribute"],
                                rowDatas["category"]
                            };
                            updateConnection.InsertValues("texts", textsValues);
                            updateConnection.InsertValues("datas", datasValues);

                            var changelogRow = new Changelog.Row()
                            {
                                Identifier = string.Format("{0} ({1})", rowTexts["name"], rowTexts["id"])
                            };

                            if (!String.IsNullOrEmpty(card.Id) && card.Id != rowTexts["id"])
                            {
                                string idUpdate = string.Format("Updated 'id': {0} -> {1}", rowTexts["id"], card.Id);
                                changelogRow.Add(idUpdate);
                            }

                            if (!String.IsNullOrEmpty(card.Name) && card.Name != rowTexts["name"])
                            {
                                string nameUpdate = string.Format("Updated 'name': \"{0}\" -> \"{1}\"", rowTexts["name"], card.Name);
                                changelogRow.Add(nameUpdate);
                            }

                            if (updatedDescription != null)
                            {
                                updatedDescription = updatedDescription.Replace("\r", "").Replace("\\r", "").Replace("\\n", "\n").Replace("\n", "  \r\n");
                            }
                            string databaseDescription = rowTexts["desc"].Replace("\r", "").Replace("\\r", "").Replace("\\n", "\n").Replace("\n", "  \r\n");

                            if (!String.IsNullOrEmpty(updatedDescription) && updatedDescription != databaseDescription)
                            {
                                TextDifference.Compute(ref databaseDescription, ref updatedDescription, new TextDifference.Format("**`-", "`**"), new TextDifference.Format("**`+", "`**"));
                                databaseDescription = databaseDescription.ReplaceOutsideChar('`', "-", "\\-").ReplaceOutsideChar('`', "[", "\\[").ReplaceOutsideChar('`', "]", "\\]");
                                updatedDescription = updatedDescription.ReplaceOutsideChar('`', "-", "\\-").ReplaceOutsideChar('`', "[", "\\[").ReplaceOutsideChar('`', "]", "\\]");

                                string descriptionDifference = string.Format("`old:` {1}  {0}`new:` {2}  ", Environment.NewLine, databaseDescription, updatedDescription);
                                changelogRow.Add("Updated card text.", descriptionDifference);
                            }

                            if (card.Appearance > 0 && card.Appearance.ToString() != rowDatas["ot"])
                            {
                                string before = databaseTranslator.ToAppearanceString(rowDatas["ot"]);
                                string after = databaseTranslator.ToAppearanceString(card.Appearance.ToString());
                                string appearanceChange = string.Format("Updated appearance: {0} -> {1}",
                                    before != null ? before : rowDatas["ot"], after != null ? after : card.Appearance.ToString());
                                changelogRow.Add(appearanceChange);
                            }

                            if (changelogRow.Count > 0)
                            {
                                changelog[databaseFileName].Changes.Add(changelogRow);
                            }
                            else
                            {
                                changelogRow = null;
                            }

                            card.Clear();
                            card = null;
                        }

                        downloadConnection.Close();
                        updateConnection.Close();
                    }
                    break;
                }
            }

            string changelogFileName = "dbupdater.log.md";
            string changelogFilePath = Path.Combine(live2016.Info.Directory, changelogFileName);

            string changelogContent = "###" + Utility.GetZuluString();

            using (var webClient = new WebClient())
            {
                foreach (var databaseFile in databaseFileNames)
                {
                    string changes = changelog[databaseFile].ToString();
                    string fileName = Path.GetFileNameWithoutExtension(databaseFile);
                    string packAbbr = fileName.Substring(fileName.IndexOf('-') + 1);
                    string packName = webClient
                        .DownloadString(string.Format("http://yugioh.wikia.com/api/v1/Search/List?query={0}&limit=1", packAbbr))?
                        .GetBetween(",\"title\":\"", "\",\"url\":\"");

                    string updateMessage = string.Format("Updated {0}.", !String.IsNullOrEmpty(packName) ? packName : databaseFile);
                    live2016.Commit(updateMessage, changes, databaseFile, "DatabaseUpdater", "yugioh.databaseupdater@gmail.com");

                    foreach (var row in changelog[databaseFile].Changes)
                        for (int i = 0; i < row.Count; ++i)
                        {
                            string node = string.Format("{0}__________________________{0}*{1}: {2}*  {0}{3}",
                                Environment.NewLine,
                                row.Identifier,
                                row[i][0],
                                (string.IsNullOrEmpty(row[i][1]) ? "" : row[i][1]));
                            changelogContent += node;
                        }

                    break;
                }
            }

            changelogContent = string.Format("{1}  {0}  {0}", Environment.NewLine, changelogContent);
            File.AppendAllText(changelogFilePath, changelogContent);

            live2016.Commit("Updated dbupdater.log.md.", null, changelogFileName, "DatabaseUpdater", "yugioh.databaseupdater@gmail.com");
            
            repositoryInfo = null;
            databaseFileNames = null;
            live2016 = null;

            var keys = changelog.Keys.ToArray();
            for (int i = 0; i < keys.Length; ++i)
            {
                changelog[keys[i]].Clear();
                changelog[keys[i]] = null;
            }

            changelog.Clear();
            changelog = null;
            /**/
        }
    }
}
