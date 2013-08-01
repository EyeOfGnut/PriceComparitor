namespace Price_Comparitor
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.fileBrowseBtn = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.progressBar1 = new NewUserAdds.Classes.StatusOverlayProgressBar();
            this.cnclBtn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.amazonServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewRtMouseClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mainBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.treeViewRtMouseClick.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // fileBrowseBtn
            // 
            this.fileBrowseBtn.Location = new System.Drawing.Point(13, 40);
            this.fileBrowseBtn.Name = "fileBrowseBtn";
            this.fileBrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.fileBrowseBtn.TabIndex = 1;
            this.fileBrowseBtn.Text = "Browse...";
            this.fileBrowseBtn.UseVisualStyleBackColor = true;
            this.fileBrowseBtn.Click += new System.EventHandler(this.fileBrowseBtn_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.Font = new System.Drawing.Font("Arial Unicode MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.ItemHeight = 22;
            this.treeView1.Location = new System.Drawing.Point(13, 69);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(449, 331);
            this.treeView1.TabIndex = 3;
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // progressBar1
            // 
            this.progressBar1.Font_Size = 8;
            this.progressBar1.Font_Style = System.Drawing.FontStyle.Bold;
            this.progressBar1.Location = new System.Drawing.Point(94, 40);
            this.progressBar1.Message = null;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Overlay_Font = new System.Drawing.Font("Arial Unicode MS", 8F, System.Drawing.FontStyle.Bold);
            this.progressBar1.Size = new System.Drawing.Size(366, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Visible = false;
            // 
            // cnclBtn
            // 
            this.cnclBtn.Location = new System.Drawing.Point(13, 40);
            this.cnclBtn.Name = "cnclBtn";
            this.cnclBtn.Size = new System.Drawing.Size(75, 23);
            this.cnclBtn.TabIndex = 6;
            this.cnclBtn.Text = "Stop";
            this.cnclBtn.UseVisualStyleBackColor = true;
            this.cnclBtn.Visible = false;
            this.cnclBtn.Click += new System.EventHandler(this.cnclBtn_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(472, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.amazonServersToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.fileToolStripMenuItem.Text = "Settings";
            // 
            // amazonServersToolStripMenuItem
            // 
            this.amazonServersToolStripMenuItem.Name = "amazonServersToolStripMenuItem";
            this.amazonServersToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.amazonServersToolStripMenuItem.Text = "Amazon Servers";
            this.amazonServersToolStripMenuItem.Click += new System.EventHandler(this.amazonServersToolStripMenuItem_Click);
            // 
            // treeViewRtMouseClick
            // 
            this.treeViewRtMouseClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.treeViewRtMouseClick.Name = "contextMenuStrip1";
            this.treeViewRtMouseClick.Size = new System.Drawing.Size(103, 26);
            // 
            // mainBindingSource
            // 
            this.mainBindingSource.DataSource = typeof(Price_Comparitor.Main);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 412);
            this.Controls.Add(this.cnclBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.fileBrowseBtn);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Price Compare";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.treeViewRtMouseClick.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button fileBrowseBtn;
        private System.Windows.Forms.BindingSource mainBindingSource;
        private System.Windows.Forms.TreeView treeView1;
        private NewUserAdds.Classes.StatusOverlayProgressBar progressBar1;
        private System.Windows.Forms.Button cnclBtn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem amazonServersToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip treeViewRtMouseClick;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}

