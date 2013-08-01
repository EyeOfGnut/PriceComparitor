using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Price_Comparitor
{
    public partial class ServerConfig : Form
    {
        public List<Server> svrs;

        public ServerConfig(List<Server> s)
        {
            InitializeComponent();
            svrs = s;
        }
        
        private void ServerConfig_Load(object sender, EventArgs e)
        {
            Point namePt = new Point(13, 13);
            int i = 1;
            foreach (Server svr in svrs)
            {
                Label l = new Label();
                l.Location = namePt;
                l.Name = "svr" + i;
                l.Text = svr.Servername;

                panel1.Controls.Add(l);

                CheckBox c = new CheckBox();
                c.Location = new Point(namePt.X + 109, namePt.Y-4);
                c.Name = "cb" + i;
                c.Checked = svr.Enabled;
                c.Text = "Enabled";
                c.Tag = svr.Servername;
               c.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);

                panel1.Controls.Add(c);

                i++;
                namePt.Y += 20;
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            svrs.Find(
                delegate(Server s)
                {
                    return s.Servername.Equals(((CheckBox)sender).Tag);
                }).Enabled = ((CheckBox)sender).Checked;
        }
    }
}
