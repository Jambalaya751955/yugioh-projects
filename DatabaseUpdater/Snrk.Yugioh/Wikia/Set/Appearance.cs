using HtmlAgilityPack;

namespace DatabaseDownloader.Wikia_
{
    partial class Set
    {
        public static int Appearance(ref HtmlDocument cardPage)
        {
            if (cardPage == null)
                return 0;

            var ocgNode = cardPage.DocumentNode.SelectSingleNode("//td[@class='cardtablespanrow']//a[@title='Yu-Gi-Oh! Official Card Game']");
            var tcgNode = cardPage.DocumentNode.SelectSingleNode("//td[@class='cardtablespanrow']//a[@title='Yu-Gi-Oh! Trading Card Game']");

            return (ocgNode != null ? 1 : 0) + (tcgNode != null ? 2 : 0);
        }
    }
}
