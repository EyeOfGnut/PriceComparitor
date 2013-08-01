using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Price_Comparitor
{
    public partial class ProfitFormula : Form
    {
        public ProfitFormula()
        {
            InitializeComponent();
        }

        private void ProfitFormula_Load(object sender, EventArgs e)
        {
            updateFields();
        }

        private void updateFields()
        {
            aTxt.Text = Price_Comparitor.Properties.Settings.Default.amazonFee.ToString();
            cTxt.Text = Price_Comparitor.Properties.Settings.Default.commission.ToString();
            fTxt.Text = Price_Comparitor.Properties.Settings.Default.fbaFee.ToString();
            tpTxt.Text = Price_Comparitor.Properties.Settings.Default.thrdPtyFee.ToString();
            pftTxt.Text = "$" + Price_Comparitor.Properties.Settings.Default.minProfit.ToString();

            profitLbl.Text = "Profit = Market - ((Market * " + cTxt.Text + ") - " + aTxt.Text + " - " + fTxt.Text + " - " + tpTxt.Text + ") - Item Cost"; 
        }

        private void setBtn_Click(object sender, EventArgs e)
        {
            /*if (Regex.Match(cTxt.Text, "[0-9]+.") == Match.Empty)
                cTxt.Text = "0" + cTxt.Text;*/

            Price_Comparitor.Properties.Settings.Default.amazonFee = float.Parse(aTxt.Text);
            Price_Comparitor.Properties.Settings.Default.commission = float.Parse(cTxt.Text);
            Price_Comparitor.Properties.Settings.Default.fbaFee = float.Parse(fTxt.Text);
            Price_Comparitor.Properties.Settings.Default.thrdPtyFee = float.Parse(tpTxt.Text);
            Price_Comparitor.Properties.Settings.Default.minProfit = float.Parse(Regex.Replace(pftTxt.Text, "\\$", ""));

            Price_Comparitor.Properties.Settings.Default.Save();
            updateFields();
        }
    }
}
