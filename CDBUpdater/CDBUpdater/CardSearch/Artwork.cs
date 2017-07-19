using System;
using System.Net;
using System.Web;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

using CDBUpdater.Helpers;

namespace CDBUpdater.CardSearch
{
    static class Artwork
    {
        public static List<string> GetUrls(List<string> ids)
        {
            if (ids == null || ids.Count < 1)
                return null;

            List<string> urls = new List<string>();
            List<Task> tasks = new List<Task>();

            foreach (string id in ids)
            {
                string url = string.Format("https://raw.githubusercontent.com/shadowfox87/YGOTCGOCGHQPics/master/{0}.jpg", id);
                tasks.Add(Task.Run(() => {
                    if (url.Exists())
                        urls.Add(url);
                }));
            }
            tasks.WaitAll();

            return urls;
        }

        private static bool Exists(this string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Head;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch { }

            return false;
        }
    }
}
