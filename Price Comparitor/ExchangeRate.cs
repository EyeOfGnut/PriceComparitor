using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Price_Comparitor
{
    using Price_Comparitor_Extensions;
    public class ExchangeRate
    {
        // USD, JPY, EUR, etc.
        public string Acronym;

        // How many of this currency to the USD. I.E., if 1 JPY = 99 USD, then this is 99.
        public float Rate;

        //Exchange rate page
        private string[] url = { "http://www.oanda.com/currency/cross-rate/result?quotes=", "&quotes=USD&go=Get+my+Table+>" };

        //Defaults to 1 if no currency Acronym is given
        public ExchangeRate()
        {
            Rate = 1;
        }

        public ExchangeRate(string acro)
            : this()
        {
            Acronym = acro;
            Rate = getERate();
        }

        // Convert given price to USD.
        public float ConvertToUSD(float price)
        {
            return price / Rate;
        }

        // Connect to the Exchange Rate webpage and grab the current exchange rate.
        private float getERate()
        {
            if (String.IsNullOrEmpty(Acronym) || Acronym.Equals("USD")) return 1;
            WebClient client = new WebClient();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(client.DownloadString(url[0]+Acronym+url[1]));

            // formatting for the table doesn't have many unique tags. 
            //The Currency row we're interested in is as far down as we get
            HtmlNode table = doc.GetElementbyId("menu_content");
            HtmlNode row = table.findChildNode("bgcolor", "#F4F4F4"); // The top row

            //The first cell in the row is the currency acronym. The second is "1.0000", the third is the number we want.
            //So we split off the extra numbers, and then trim off everything that's NOT a number.
            string rString = row.InnerText.Substring(row.InnerText.IndexOf("1.0000") + 10);
            rString = Regex.Replace(rString, "[^0-9\\.]", "").Trim();
            rString = Regex.Replace(rString, "^\\.", "").Trim();// kill any accidentally left leading .'s
            return float.Parse(rString);
        }
    }
}
