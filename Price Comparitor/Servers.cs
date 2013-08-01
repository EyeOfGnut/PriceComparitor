
namespace Price_Comparitor
{
    /// <summary>
    /// Information for a specific Amazon server. I.E., amazon.com, amazon.co.jp, etc.
    /// </summary>
    public class Server
    {
        public string SearchUrl; // The whole URL for the search, minus the SKU
        public float Fee; // Fee specific to this version of Amazon
        public string Servername; // www.amazon.com, etc.
        public ExchangeRate Exchange; // Exhange object for foreign Amazon servers.
        public bool Enabled = true;

        public Server() 
        {
        }

        public Server(string svr, string srch, float fee) 
            : this()
        {
            SearchUrl = srch;
            Fee = fee;
            Servername = svr;
            Exchange = new ExchangeRate();
        }

        public Server(string svr, string srch, float fee, string acronym)
        {
            SearchUrl = srch;
            Fee = fee;
            Servername = svr;
            Exchange = new ExchangeRate(acronym);
        }

        public Server(string svr, string srch, float fee, string acronym, bool e)
            : this(svr, srch, fee, acronym)
        {
            Enabled = e;
        }
    }
}
