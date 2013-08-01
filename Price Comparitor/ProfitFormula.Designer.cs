namespace Price_Comparitor
{
    partial class ProfitFormula
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comLbl = new System.Windows.Forms.Label();
            this.aFeeLbl = new System.Windows.Forms.Label();
            this.fbaFee = new System.Windows.Forms.Label();
            this.tpFeeLbl = new System.Windows.Forms.Label();
            this.cTxt = new System.Windows.Forms.TextBox();
            this.aTxt = new System.Windows.Forms.TextBox();
            this.fTxt = new System.Windows.Forms.TextBox();
            this.tpTxt = new System.Windows.Forms.TextBox();
            this.setBtn = new System.Windows.Forms.Button();
            this.profitLbl = new System.Windows.Forms.Label();
            this.minTxt = new System.Windows.Forms.Label();
            this.pftTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comLbl
            // 
            this.comLbl.AutoSize = true;
            this.comLbl.Location = new System.Drawing.Point(13, 16);
            this.comLbl.Name = "comLbl";
            this.comLbl.Size = new System.Drawing.Size(62, 13);
            this.comLbl.TabIndex = 0;
            this.comLbl.Text = "Commission";
            // 
            // aFeeLbl
            // 
            this.aFeeLbl.AutoSize = true;
            this.aFeeLbl.Location = new System.Drawing.Point(13, 50);
            this.aFeeLbl.Name = "aFeeLbl";
            this.aFeeLbl.Size = new System.Drawing.Size(66, 13);
            this.aFeeLbl.TabIndex = 1;
            this.aFeeLbl.Text = "Amazon Fee";
            // 
            // fbaFee
            // 
            this.fbaFee.AutoSize = true;
            this.fbaFee.Location = new System.Drawing.Point(15, 80);
            this.fbaFee.Name = "fbaFee";
            this.fbaFee.Size = new System.Drawing.Size(48, 13);
            this.fbaFee.TabIndex = 2;
            this.fbaFee.Text = "FBA Fee";
            // 
            // tpFeeLbl
            // 
            this.tpFeeLbl.AutoSize = true;
            this.tpFeeLbl.Location = new System.Drawing.Point(15, 106);
            this.tpFeeLbl.Name = "tpFeeLbl";
            this.tpFeeLbl.Size = new System.Drawing.Size(70, 13);
            this.tpFeeLbl.TabIndex = 3;
            this.tpFeeLbl.Text = "3rd Party Fee";
            // 
            // cTxt
            // 
            this.cTxt.Location = new System.Drawing.Point(91, 13);
            this.cTxt.Name = "cTxt";
            this.cTxt.Size = new System.Drawing.Size(100, 20);
            this.cTxt.TabIndex = 4;
            // 
            // aTxt
            // 
            this.aTxt.Location = new System.Drawing.Point(91, 47);
            this.aTxt.Name = "aTxt";
            this.aTxt.Size = new System.Drawing.Size(100, 20);
            this.aTxt.TabIndex = 5;
            // 
            // fTxt
            // 
            this.fTxt.Location = new System.Drawing.Point(91, 77);
            this.fTxt.Name = "fTxt";
            this.fTxt.Size = new System.Drawing.Size(100, 20);
            this.fTxt.TabIndex = 6;
            // 
            // tpTxt
            // 
            this.tpTxt.Location = new System.Drawing.Point(91, 103);
            this.tpTxt.Name = "tpTxt";
            this.tpTxt.Size = new System.Drawing.Size(100, 20);
            this.tpTxt.TabIndex = 7;
            // 
            // setBtn
            // 
            this.setBtn.Location = new System.Drawing.Point(18, 129);
            this.setBtn.Name = "setBtn";
            this.setBtn.Size = new System.Drawing.Size(75, 23);
            this.setBtn.TabIndex = 8;
            this.setBtn.Text = "Save";
            this.setBtn.UseVisualStyleBackColor = true;
            this.setBtn.Click += new System.EventHandler(this.setBtn_Click);
            // 
            // profitLbl
            // 
            this.profitLbl.AutoSize = true;
            this.profitLbl.Location = new System.Drawing.Point(18, 186);
            this.profitLbl.Name = "profitLbl";
            this.profitLbl.Size = new System.Drawing.Size(31, 13);
            this.profitLbl.TabIndex = 9;
            this.profitLbl.Text = "Profit";
            // 
            // minTxt
            // 
            this.minTxt.AutoSize = true;
            this.minTxt.Location = new System.Drawing.Point(253, 15);
            this.minTxt.Name = "minTxt";
            this.minTxt.Size = new System.Drawing.Size(121, 13);
            this.minTxt.TabIndex = 10;
            this.minTxt.Text = "Minimum Profit Required";
            // 
            // pftTxt
            // 
            this.pftTxt.Location = new System.Drawing.Point(256, 31);
            this.pftTxt.Name = "pftTxt";
            this.pftTxt.Size = new System.Drawing.Size(100, 20);
            this.pftTxt.TabIndex = 11;
            // 
            // ProfitFormula
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 221);
            this.Controls.Add(this.pftTxt);
            this.Controls.Add(this.minTxt);
            this.Controls.Add(this.profitLbl);
            this.Controls.Add(this.setBtn);
            this.Controls.Add(this.tpTxt);
            this.Controls.Add(this.fTxt);
            this.Controls.Add(this.aTxt);
            this.Controls.Add(this.cTxt);
            this.Controls.Add(this.tpFeeLbl);
            this.Controls.Add(this.fbaFee);
            this.Controls.Add(this.aFeeLbl);
            this.Controls.Add(this.comLbl);
            this.Name = "ProfitFormula";
            this.Text = "ProfitFormula";
            this.Load += new System.EventHandler(this.ProfitFormula_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label comLbl;
        private System.Windows.Forms.Label aFeeLbl;
        private System.Windows.Forms.Label fbaFee;
        private System.Windows.Forms.Label tpFeeLbl;
        private System.Windows.Forms.TextBox cTxt;
        private System.Windows.Forms.TextBox aTxt;
        private System.Windows.Forms.TextBox fTxt;
        private System.Windows.Forms.TextBox tpTxt;
        private System.Windows.Forms.Button setBtn;
        private System.Windows.Forms.Label profitLbl;
        private System.Windows.Forms.Label minTxt;
        private System.Windows.Forms.TextBox pftTxt;
    }
}