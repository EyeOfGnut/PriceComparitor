namespace Price_Comparitor
{
    partial class ColumnSelect
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
            this.dataPanel = new System.Windows.Forms.Panel();
            this.okBtn = new System.Windows.Forms.Button();
            this.lb1 = new System.Windows.Forms.Label();
            this.cb1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataPanel
            // 
            this.dataPanel.Controls.Add(this.label2);
            this.dataPanel.Controls.Add(this.label1);
            this.dataPanel.Controls.Add(this.okBtn);
            this.dataPanel.Controls.Add(this.lb1);
            this.dataPanel.Controls.Add(this.cb1);
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPanel.Location = new System.Drawing.Point(0, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(373, 279);
            this.dataPanel.TabIndex = 0;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(286, 244);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "Ok";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Location = new System.Drawing.Point(156, 43);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(35, 13);
            this.lb1.TabIndex = 1;
            this.lb1.Text = "label1";
            this.lb1.Visible = false;
            // 
            // cb1
            // 
            this.cb1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb1.FormattingEnabled = true;
            this.cb1.Items.AddRange(new object[] {
            "[Ignore]",
            "Item Name",
            "SKU",
            "Price"});
            this.cb1.Location = new System.Drawing.Point(12, 40);
            this.cb1.MaxDropDownItems = 4;
            this.cb1.Name = "cb1";
            this.cb1.Size = new System.Drawing.Size(121, 21);
            this.cb1.TabIndex = 0;
            this.cb1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Column Designation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(156, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sample Data";
            // 
            // ColumnSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 279);
            this.Controls.Add(this.dataPanel);
            this.Name = "ColumnSelect";
            this.Text = "Define Column Data";
            this.Load += new System.EventHandler(this.ColumnSelect_Load);
            this.dataPanel.ResumeLayout(false);
            this.dataPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel dataPanel;
        private System.Windows.Forms.ComboBox cb1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

    }
}