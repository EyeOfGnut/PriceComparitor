using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Price_Comparitor_Extensions;
using System.Threading;

namespace Price_Comparitor
{
    class Product
    {
        private const string CLASSKEY = "class";
        private const string IMGKEY = "src";
        private const string HREFKEY = "href";

        private const string HTMLERROR = "HTML_Error";
        //private const string headers_accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"; //pulled from FireBug
        //private const string headers_agent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; de; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12";

        private const string headers_agent = "Lynx/2.8.5rel.1 libwww-FM/2.14"; // emulate Lynx - text only, no ssl.
        private const string headers_accept = "text/plain";


        //private string headers_mobile_agent = "Mozilla/5.0 (Linux; U; Android 3.0; en-us; Xoom Build/HRI39) AppleWebKit/525.10 (KHTML, like Gecko) Version/3.0.4 Mobile Safari/523.12.2";
        
        private StringBuilder csvString;
        private CancellationToken cancel;

        public string sku
        {
            get 
            {
                return (String.IsNullOrEmpty(_sku) || Regex.IsMatch(_sku, "[a-zA-Z]+?")) ? _name : _sku;
            }
            set 
            {
                _sku = (String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(_name)) ? _name : value;
                checkDone();
            }
        }
        private string _sku;

        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                checkDone();
            }
        }
        private string _name;

        public string price
        {
            get {return _price;}
            set
            {
                _price = value;
                checkDone();
            }
        }
        private string _price;

        public List<TreeNode> subTree;
        public bool done;
        public uint? RowNumber; // Nullable

        public class ProductInfo {
            public String Preferred;
            public String Other;
            public String Rank; 
        }
        public Dictionary<String, ProductInfo> ServerPrices; 

        public Product(CancellationToken cncl)
        {
            subTree = new List<TreeNode>();
            done = false;
            cancel = cncl;
            csvString = new StringBuilder();
            ServerPrices = new Dictionary<string, ProductInfo>();
        }

        public Product(string skuNum, CancellationToken cncl)
            : this(cncl)
        {
            sku = skuNum;
        }

        public Product(string skuNum, string nameStr, CancellationToken cncl)
            : this(skuNum, cncl)
        {
            name = nameStr;
        }

        /// <summary>
        /// Do we have all the required variables assigned??
        /// </summary>
        private void checkDone()
        {
            done = !string.IsNullOrEmpty(_sku) && !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_price);
        }

        /// <summary>
        /// Connect to the Amazon servers to get the search results, and product information.
        /// </summary>
        /// <param name="svrs">List of Servers - this will connect to each one for results.</param>
        public void fillResults(List<Server> svrs)
        {
            if (done)
            {
                List<TreeNode> c = new List<TreeNode>(); // Need this for Async processing
                // Parallel processing, Baby!
                // Dowload the product for each server Asyncronously.
                
                Parallel.ForEach(svrs, new Action<Server, ParallelLoopState>((Server svr, ParallelLoopState state) =>
                {
                    if (cancel.IsCancellationRequested) state.Break();

                    TreeNode resultsNode = buildResultsNode(svr);
                    if (resultsNode != null)
                    {
                        resultsNode.Name = svr.Servername;
                        lock (c) c.Add(resultsNode);
                    }
                }));

                // Add the results to the Kids list.
                if (c.Count > 0)
                {
                    lock (subTree)
                    {
                        foreach (TreeNode kid in c)
                        {
                            if (kid != null)
                                subTree.Add(kid);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build the results node.
        /// +ServerName
        ///  |- Result 1 details
        ///  |- Result 2 details
        ///  |-...
        /// </summary>
        /// <param name="svr">Server object to search</param>
        /// <returns>returns the completed node, to add to the treeview</returns>
        private TreeNode buildResultsNode(Server svr)
        {
            TreeNode[] nodeArray = checkAmazon(svr);

            // Don't add a node for the Amazon server if the SKU isn't found
            return (nodeArray != null && !cancel.IsCancellationRequested) ? new TreeNode(svr.Servername, nodeArray) : null;
        }

        /// <summary>
        /// Fill an HtmlAgilityPack HtmlDocument with data from the specified URL
        /// </summary>
        /// <param name="url">Address of the page to download</param>
        /// <returns>The filled Document</returns>
        private HtmlAgilityPack.HtmlDocument loadHtmlDocument(String url)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Accept] = headers_accept;
                client.Headers[HttpRequestHeader.UserAgent] = headers_agent;
                try
                {
                    doc.LoadHtml(client.DownloadString(url)); // Do the search
                }
                catch (Exception ex)
                {
                    /*MessageBox.Show("Unable to download " + url + "\n" + ex.Message, "Error Downloading Page",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);*/
                    HtmlAgilityPack.HtmlDocument d = new HtmlAgilityPack.HtmlDocument();
                    d.CreateAttribute(HTMLERROR, ex.ToString());
                    return d;
                }
            }
            return doc;
        }

        private List<TreeNode> fillPrices(HtmlNode nd, Server svr, ProductInfo info)
        {
            List<TreeNode> results = new List<TreeNode>();

                if (nd != null && !cancel.IsCancellationRequested)
                {
                    List<Competitor> cList = getResellers(nd) ?? new List<Competitor>();
                    foreach (Competitor c in cList)
                    {
                        if (c.Name.Contains("Error"))
                            results.Add(new TreeNode(c.Name));
                        else
                        {
                            float cPrice = svr.Exchange.ConvertToUSD(c.Price);
                            float cShipping = svr.Exchange.ConvertToUSD(c.Shipping);

                            if (c.Preferred)
                            {
                                info.Preferred = (cPrice + cShipping).ToString("C2");
                                results.Add(createSubNode("Preferred", info.Preferred, info.Rank));
                            }
                            else
                            {
                                info.Other = (cPrice + cShipping).ToString("C2");
                                results.Add(createSubNode("Normal", info.Other, info.Rank));
                            }
                        }
                    }
                }

            return results;
        }

        /// <summary>
        /// Search Amazon for the item's SKU
        /// </summary>
        /// <param name="svr">The Server object, containing the information for the specific Amazon server</param>
        /// <param name="price">The _VENDOR'S_ price for the item, to compare against Amazon's</param>
        /// <returns>A TreeNode[] array containing info details for that server's search results</returns>
        private TreeNode[] checkAmazon(Server svr)
        {
            HtmlAgilityPack.HtmlDocument doc = loadHtmlDocument(svr.SearchUrl + sku);

            HtmlNode node = doc.GetElementbyId("result_0"); // Get the first search result. This will be NULL if there were no results.
            List<TreeNode> results = null; ;
            ProductInfo info = new ProductInfo();

            if (node != null)
            {
                info.Rank = getRank(node, sku);

                HtmlNode p = node.findChildNode(HREFKEY, "condition=new", false); // This is the price on the search results page
                if (p != null && (Regex.Replace(p.InnerText, "[^0-9\\.]", "").Trim()).Length > 0)
                {
                    results = fillPrices(p, svr, info); //Trim all the garbage out, so we have just the price
                }
            }
            else
            {
                node = doc.GetElementbyId(HTMLERROR);
                if (node != null)
                {
                    if (null == results) results = new List<TreeNode>();
                    results.Add(new TreeNode(node.InnerText));
                }
            }

            if (null == results || results.Count == 0) return null; //Return null if no results
            else
            {
                this.ServerPrices.Add(svr.Servername, info);
                return results.ToArray(); // And we're done.
            }
        }

        private TreeNode createSubNode(String sellerType, string price, String rating)
        {
            TreeNode node = new TreeNode(sellerType + " Reseller: " + price + " - " + rating);
            node.Name = sellerType;
            return node;
        }

        private List<Competitor> getResellers(HtmlNode nd)
        {
            string url = nd.OuterHtml.Substring(nd.OuterHtml.IndexOf('\"') + 1);
            List<Competitor> cList = findResellers(url.Substring(0, url.IndexOf('\"')));

            // Try to get the shipping one extra time.
            if (cList != null && cList.Find(delegate(Competitor comp) { return comp.Rerun; }) != null)
            {
                Console.WriteLine("Retrying " + url.Substring(0, url.IndexOf('\"')));
                cList = findResellers(url.Substring(0, url.IndexOf('\"')));
            }

            return cList;
        }

        /// <summary>
        /// Sort the resellers to find the lowest overall price
        /// </summary>
        /// <param name="competitors">List of Competitor objects to sort</param>
        /// <returns>List of one or two competitors, depending on if a preferred seller has the lowest price or not</returns>
        private List<Competitor> sortedResellers(List<Competitor> competitors)
        {
            Competitor lowest = null;
            Competitor primeNotLowest = null;

            foreach (Competitor c in competitors)
            {
                if (lowest == null)
                    lowest = c;
                else
                {
                    if (c.Price + c.Shipping < lowest.Price + lowest.Shipping)
                    {
                        if (lowest.Preferred)
                            primeNotLowest = lowest;

                        lowest = c;
                    }
                    else
                    {
                        if (c.Preferred)
                        {
                            if (primeNotLowest == null)
                                primeNotLowest = c;
                            else
                            {
                                if (c.Price + c.Shipping < primeNotLowest.Price + primeNotLowest.Shipping)
                                    primeNotLowest = c;
                            }
                        }
                    }
                }
            }

            List<Competitor> cList = new List<Competitor>();
            cList.Add(lowest);
            if (!lowest.Preferred && primeNotLowest != null) cList.Add(primeNotLowest);

            return cList;
        }

        private HtmlNode getVendorPage(string sellersUrl)
        {
#if DEBUG
            Console.WriteLine("Downloading from\n\t" + sellersUrl);
#endif

            HtmlAgilityPack.HtmlDocument doc = loadHtmlDocument(sellersUrl);
            return doc.GetElementbyId("olpOfferList"); //Overall results container
        }

        /// <summary>
        /// Sift through the list of "alternate" sellers for an item, and pull the information for each seller (price, shippng, etc). Returns null if none found.
        /// </summary>
        /// <param name="client">Web Client object</param>
        /// <param name="sellersUrl">URL of the list of sellers</param>
        /// <returns>List of Competitor objects, representing each seller.</returns>
        private List<Competitor> findResellers(string sellersUrl)
        {
            List<Competitor> competitors = new List<Competitor>();

            HtmlNode starter = getVendorPage(sellersUrl);
            if (starter == null)
            {
                Competitor c = new Competitor();
                c.Price = 0;
                c.Shipping = 0;
                c.Name = "Error from server on " + sellersUrl;
                competitors.Add(c);
                return competitors;
            }

            List<HtmlNode> nodeList = starter.findAllChildNodes(CLASSKEY, "olpOffer", false);

            Parallel.ForEach(nodeList, new Action<HtmlNode, ParallelLoopState>((HtmlNode node, ParallelLoopState state) => 
            {
                if (cancel.IsCancellationRequested) state.Break();

                HtmlNode n = node.findChildNode(CLASSKEY, "olpCondition", false);
                if (n != null)
                {
                    if (!n.InnerHtml.Contains("Used")) // Skip the node if it's Used
                    {
                        Competitor c = new Competitor();

                        //Get the price from the page
                        c.Price = findPrice(n, competitors);

                        //Get the Shipping from the page
                        c.Shipping = findShippingInfo(n, competitors);

                        // Get Preferred status of the reseller
                        c.Preferred = (node.findChildNode(IMGKEY, "sip-prime-check-badge", false) != null);

                        // Get URL of the reseller's logo
                        c.LogoURL = findResellerLogo(n);

                        // Add the info to the list of competitors
                        lock (competitors) competitors.Add(c);
                    }
                }
            }));
            if (competitors == null || competitors.Count < 1) return null;
            else return sortedResellers(competitors);
        }

        private float findPrice(HtmlNode node, List<Competitor> competitors)
        {
            HtmlNode n = node.findChildNode(CLASSKEY, "olpOfferPrice", false);
            if (n != null)
            {
                float price = parsePrice(n.InnerHtml);
                if (price < 0)
                {
                    Console.WriteLine("Error getting Price");
                    Competitor rerun = new Competitor();
                    rerun.Rerun = true;
                    lock (competitors) competitors.Add(rerun);
                }
                else
                {
                    return price;
                }
            }

            return 0;
        }

        private float findShippingInfo(HtmlNode node, List<Competitor> competitors)
        {
            HtmlNode n = node.findChildNode(CLASSKEY, "olpShippingPrice", true);
            if (n != null)
            {
                float shipping = parsePrice(n.InnerHtml);
                if (shipping < 0)
                {
                    Console.WriteLine("Error getting Shipping");
                    Competitor rerun = new Competitor();
                    rerun.Rerun = true;
                    lock (competitors) competitors.Add(rerun);
                }
                else
                {
                     return shipping;
                }
            }

            return 0;
        }

        private string findResellerLogo(HtmlNode node)
        {
            HtmlNode n = node.findChildNode(IMGKEY, "ecx.images-amazon.com/images/I/", false);
            if (n != null)
            {
                string url = n.OuterHtml.Substring(n.OuterHtml.IndexOf("http"));
                return url.Substring(0, url.IndexOf(' ') - 1);
            }
            return string.Empty;
        }

        private float parsePrice(string price)
        {
            try
            {
                return float.Parse(Regex.Replace(price, "[^0-9\\.]", "").Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading " + sku + "\n" + ex.Message);
                return -1000;
            }
        }

        private string getRank(HtmlNode node, string sku)
        {
            // Find the Hyperlink, which contains the SKU.
            // Since the node is a wrapper around an individual search result, the Hyperlink will be the only one there.
            HtmlNode n = node.findChildNode("href", sku, false);

            if (n != null) // if the link is found
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                String href = n.GetAttributeValue("href", "");



                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.Accept] = headers_accept;
                    client.Headers[HttpRequestHeader.UserAgent] = headers_agent;
                    try
                    {
                        doc.LoadHtml(client.DownloadString(href)); // Download the product detail page
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(href + " :\n" + ex.Message);
                        return "Rank Error - Can't load item page";
                    }
                }

                HtmlNode rankNode = doc.GetElementbyId("SalesRank"); //Need to trim a TON of \n
                StringBuilder sb = new StringBuilder();

                // Parse the seller's rating
                if (rankNode != null)
                {
                    //Get rid of all the text but the Seller's rank (there's a lot of crap surrounding it)

                    String rank;
                    if (rankNode.InnerText.Contains('('))
                        rank = Regex.Replace(rankNode.InnerText.Substring(0, rankNode.InnerText.IndexOf('(')), "[^0-9]", "").Trim();
                    else
                        rank = Regex.Replace(rankNode.InnerText, "[^0-9]", "").Trim();


                    if (rank.Length > 0)
                        return rank;
                }
            }
            return "Rank Error - can't find the link";
        }
    }
}
