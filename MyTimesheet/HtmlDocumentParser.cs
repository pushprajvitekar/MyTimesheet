using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyTimesheet
{
    public class HtmlDocumentParser
    {
        public static IEnumerable<string[]> ParseHtmlFile(string fileText)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(fileText);
            var nodes = doc.DocumentNode.SelectNodes("//table/tr");
           
            var rows = nodes.Skip(2).Select(tr => tr
                .Elements("td")
                .Select(td => HttpUtility.HtmlDecode(td.InnerText.Trim()))
                .ToArray());
            return rows;
        }
    }
}
