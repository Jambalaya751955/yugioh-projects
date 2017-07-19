using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace Snrk.Downloaders.Github
{
    public class GithubDownloader : IDisposable
    {
        public readonly string RepositoryUrl;

        private HtmlDocument m_RepositoryPage;
        private WebClient m_WebClient;

        public GithubDownloader(string repositoryUrl)
        {
            m_WebClient = new WebClient();
            this.RepositoryUrl = repositoryUrl.TrimEnd('/');

            HtmlWeb htmlWeb = new HtmlWeb();
            try { m_RepositoryPage = htmlWeb.Load(this.RepositoryUrl); } catch { }
            htmlWeb = null;
        }

        ~GithubDownloader() { Dispose(); }
        public void Dispose()
        {
            m_RepositoryPage = null;
            m_WebClient?.Dispose();
            m_WebClient = null;
        }

        public bool DownloadFiles(string directory, Predicate<string> fileNamePredicate)
        {
            if (!Directory.Exists(directory))
                try { Directory.CreateDirectory(directory); } catch { return false; }

            HtmlNodeCollection fileNodes = m_RepositoryPage.DocumentNode.SelectNodes("//tr[@class='js-navigation-item']//a[@class='js-navigation-open']");
            foreach (HtmlNode file in fileNodes)
            {
                string fileName = file.InnerText;
                string fileUrl = "https://github.com" + file.Attributes["href"].Value;
                if (fileNamePredicate(fileName))
                {
                    string savingDirectory = Path.Combine(directory, fileName);
                    string rawFileUrl = fileUrl
                        .Replace("https://github.com", "https://raw.githubusercontent.com")
                        .Replace("/blob", string.Empty);
                    try { m_WebClient.DownloadFile(rawFileUrl, savingDirectory); } catch { }
                }
            }
            fileNodes = null;

            return true;
        }
    }
}
