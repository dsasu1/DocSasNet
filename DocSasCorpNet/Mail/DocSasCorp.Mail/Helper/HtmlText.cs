using HtmlAgilityPack;
using System;
using System.IO;

namespace DocSasCorp.Mail.Helper
{
    /// <summary>
    /// HtmlText
    /// </summary>
    public class HtmlText
    {
        /// <summary>
        /// ConvertHtml
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ConvertHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        /// <summary>
        /// ConvertContentTo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="outText"></param>
        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        /// <summary>
        /// ConvertTo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="outText"></param>
        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    var parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    var html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    if (node.Name == "p")
                        outText.Write("\r\n");

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}