namespace VideoLogoBatch
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            menuStrip1 = new MenuStrip();
            fIleToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            saveConfigToolStripMenuItem = new ToolStripMenuItem();
            loadConfigToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            videoSourceControl1 = new VideoSourceControl();
            logoOptionsControl1 = new LogoOptionsControl();
            previewControl1 = new PreviewControl();
            renderControl1 = new RenderControl();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Font = new Font("Segoe UI", 10F);
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fIleToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1262, 31);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fIleToolStripMenuItem
            // 
            fIleToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, openFolderToolStripMenuItem, saveConfigToolStripMenuItem, loadConfigToolStripMenuItem, exitToolStripMenuItem });
            fIleToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            fIleToolStripMenuItem.Name = "fIleToolStripMenuItem";
            fIleToolStripMenuItem.Size = new Size(49, 27);
            fIleToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(188, 28);
            openFileToolStripMenuItem.Text = "Open File";
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new Size(188, 28);
            openFolderToolStripMenuItem.Text = "Open Folder";
            // 
            // saveConfigToolStripMenuItem
            // 
            saveConfigToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            saveConfigToolStripMenuItem.Name = "saveConfigToolStripMenuItem";
            saveConfigToolStripMenuItem.Size = new Size(188, 28);
            saveConfigToolStripMenuItem.Text = "Save Config";
            // 
            // loadConfigToolStripMenuItem
            // 
            loadConfigToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            loadConfigToolStripMenuItem.Name = "loadConfigToolStripMenuItem";
            loadConfigToolStripMenuItem.Size = new Size(188, 28);
            loadConfigToolStripMenuItem.Text = "Load Config";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(188, 28);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Font = new Font("Segoe UI", 10F);
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(59, 27);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(141, 28);
            aboutToolStripMenuItem.Text = "About";
            // 
            // videoSourceControl1
            // 
            videoSourceControl1.Location = new Point(12, 34);
            videoSourceControl1.Name = "videoSourceControl1";
            videoSourceControl1.Size = new Size(340, 627);
            videoSourceControl1.TabIndex = 7;
            // 
            // logoOptionsControl1
            // 
            logoOptionsControl1.Location = new Point(348, 449);
            logoOptionsControl1.Name = "logoOptionsControl1";
            logoOptionsControl1.Size = new Size(902, 212);
            logoOptionsControl1.TabIndex = 8;
            // 
            // previewControl1
            // 
            previewControl1.Location = new Point(348, 34);
            previewControl1.Name = "previewControl1";
            previewControl1.Size = new Size(670, 409);
            previewControl1.TabIndex = 9;
            // 
            // renderControl1
            // 
            renderControl1.Location = new Point(1014, 34);
            renderControl1.Name = "renderControl1";
            renderControl1.Size = new Size(236, 409);
            renderControl1.TabIndex = 10;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1262, 673);
            Controls.Add(renderControl1);
            Controls.Add(previewControl1);
            Controls.Add(logoOptionsControl1);
            Controls.Add(videoSourceControl1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Main";
            Text = "Video Logo Batch";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fIleToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripMenuItem saveConfigToolStripMenuItem;
        private ToolStripMenuItem loadConfigToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private VideoSourceControl videoSourceControl1;
        private LogoOptionsControl logoOptionsControl1;
        private PreviewControl previewControl1;
        private RenderControl renderControl1;
    }
}
