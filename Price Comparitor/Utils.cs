using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;

namespace Price_Comparitor_Extensions
{
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
                webRequest.CookieContainer = m_container;

            return request;
        }
    }

    public static class HtmlNodeExtension
    {
        /// <summary>
        /// Search the chile nodes for the given pattern
        /// </summary>
        /// <param name="cursor">The node to search</param>
        /// <param name="attribute">Attribute to searh ("class", "href", etc)</param>
        /// <param name="value">Exact value of the Attribute, specific to the node we're looking for, without partial matching</param>
        /// <returns></returns>
        public static HtmlNode findChildNode(this HtmlNode cursor, String attribute, String value)
        {
            return findChildNode(cursor, attribute, value, true);
        }

        /// <summary>
        /// Search the child nodes for the given pattern
        /// </summary>
        /// <param name="cursor">The node to search</param>
        /// <param name="attribute">Attribute to searh ("class", "href", etc)</param>
        /// <param name="value">Value of the Attribute, specific to the node we're looking for</param>
        /// <param name="exact">Search for exact match or partial (attribute IS the Value string, or CONTAINS the Value string). Defaults to Exact</param>
        /// <returns>The First child node found that matches the pattern</returns>
        public static HtmlNode findChildNode(this HtmlNode cursor, String attribute, String value, Boolean exact)
        {
            if (cursor == null)
            {
                //Console.WriteLine("Error finding Value : " + value + " - parent node is NULL");
                return null;
            }
            HtmlNode foundNode = null;

            if (exact)
            {
                if (cursor.GetAttributeValue(attribute, "").Equals(value))
                    return cursor;
            }
            else
            {
                if (cursor.GetAttributeValue(attribute, "").Contains(value))
                    return cursor;
            }

            if (cursor.HasChildNodes)
            {
                cursor = cursor.FirstChild;
                while (cursor != null && foundNode == null)
                {
                    foundNode = findChildNode(cursor, attribute, value, exact);
                    cursor = cursor.NextSibling;
                }
            }
            return foundNode;
        }

        public static List<HtmlNode> findAllChildNodes(this HtmlNode cursor, String attribute, String value)
        {
            return findAllChildNodes(cursor, attribute, value, true);
        }

        public static List<HtmlNode> findAllChildNodes(this HtmlNode cursor, String attribute, String value, Boolean exact)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();

            while (cursor != null)
            {
                if (exact)
                {
                    if (cursor.GetAttributeValue(attribute, "").Equals(value))
                        nodes.Add(cursor);
                }
                else
                {
                    if (cursor.GetAttributeValue(attribute, "").Contains(value))
                        nodes.Add(cursor);
                }


                if (cursor.HasChildNodes)
                    nodes.AddRange(findAllChildNodes(cursor.FirstChild, attribute, value, exact));

                cursor = cursor.NextSibling;
            }
            return nodes;
        }
    }

    public class Utils
    {
        /// <summary>
        /// Profit calculation (Market - ((Market*Commission)-Fees) - Vendor Price)
        /// </summary>
        /// <param name="marketPrice">Price on Amazon</param>
        /// <param name="itemPrice">Price from Vendor</param>
        /// <returns>Float profit amount</returns>
        public static float Profit(float marketPrice, float itemPrice)
        {
            float commision = Price_Comparitor.Properties.Settings.Default.commission;
            float aFee = Price_Comparitor.Properties.Settings.Default.amazonFee;
            float fbaFee = Price_Comparitor.Properties.Settings.Default.fbaFee;
            float tpFee = Price_Comparitor.Properties.Settings.Default.thrdPtyFee;

            float profit = marketPrice-((marketPrice*commision)-aFee-fbaFee-tpFee)-itemPrice;
            return profit;
        }
    }
}
