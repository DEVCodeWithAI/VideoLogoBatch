namespace VideoLogoBatch
{
    partial class VideoSourceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBox3 = new GroupBox();
            button_trim_cut = new Button();
            button_select_all_video_list = new Button();
            button_delete_select_video_list = new Button();
            button_clean_video_list = new Button();
            listBox_video_source = new ListBox();
            button_import_folder = new Button();
            button_import_file = new Button();
            contextMenuStripVideoItem = new ContextMenuStrip(components);
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            groupBox3.SuspendLayout();
            contextMenuStripVideoItem.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button_trim_cut);
            groupBox3.Controls.Add(button_select_all_video_list);
            groupBox3.Controls.Add(button_delete_select_video_list);
            groupBox3.Controls.Add(button_clean_video_list);
            groupBox3.Controls.Add(listBox_video_source);
            groupBox3.Controls.Add(button_import_folder);
            groupBox3.Controls.Add(button_import_file);
            groupBox3.Location = new Point(2, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(330, 618);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Video Source";
            // 
            // button_trim_cut
            // 
            button_trim_cut.Location = new Point(169, 570);
            button_trim_cut.Name = "button_trim_cut";
            button_trim_cut.Size = new Size(140, 29);
            button_trim_cut.TabIndex = 6;
            button_trim_cut.Text = "Trim CUT";
            button_trim_cut.UseVisualStyleBackColor = true;
            // 
            // button_select_all_video_list
            // 
            button_select_all_video_list.Location = new Point(169, 535);
            button_select_all_video_list.Name = "button_select_all_video_list";
            button_select_all_video_list.Size = new Size(140, 29);
            button_select_all_video_list.TabIndex = 5;
            button_select_all_video_list.Text = "Select All";
            button_select_all_video_list.UseVisualStyleBackColor = true;
            button_select_all_video_list.Click += Button_select_all_video_list_Click;
            // 
            // button_delete_select_video_list
            // 
            button_delete_select_video_list.Location = new Point(23, 568);
            button_delete_select_video_list.Name = "button_delete_select_video_list";
            button_delete_select_video_list.Size = new Size(140, 29);
            button_delete_select_video_list.TabIndex = 4;
            button_delete_select_video_list.Text = "Delete Select";
            button_delete_select_video_list.UseVisualStyleBackColor = true;
            button_delete_select_video_list.Click += Button_delete_select_video_list_Click;
            // 
            // button_clean_video_list
            // 
            button_clean_video_list.Location = new Point(23, 535);
            button_clean_video_list.Name = "button_clean_video_list";
            button_clean_video_list.Size = new Size(140, 29);
            button_clean_video_list.TabIndex = 3;
            button_clean_video_list.Text = "Clean List";
            button_clean_video_list.UseVisualStyleBackColor = true;
            button_clean_video_list.Click += Button_clean_video_list_Click;
            // 
            // listBox_video_source
            // 
            listBox_video_source.ContextMenuStrip = contextMenuStripVideoItem;
            listBox_video_source.FormattingEnabled = true;
            listBox_video_source.Location = new Point(6, 85);
            listBox_video_source.Name = "listBox_video_source";
            listBox_video_source.SelectionMode = SelectionMode.MultiExtended;
            listBox_video_source.Size = new Size(318, 444);
            listBox_video_source.TabIndex = 2;
            // 
            // button_import_folder
            // 
            button_import_folder.Location = new Point(169, 39);
            button_import_folder.Name = "button_import_folder";
            button_import_folder.Size = new Size(121, 29);
            button_import_folder.TabIndex = 1;
            button_import_folder.Text = "Import Folder";
            button_import_folder.UseVisualStyleBackColor = true;
            button_import_folder.Click += Button_import_folder_Click;
            // 
            // button_import_file
            // 
            button_import_file.Location = new Point(42, 39);
            button_import_file.Name = "button_import_file";
            button_import_file.Size = new Size(121, 29);
            button_import_file.TabIndex = 0;
            button_import_file.Text = "Import Video";
            button_import_file.UseVisualStyleBackColor = true;
            button_import_file.Click += Button_import_file_Click;
            // 
            // contextMenuStripVideoItem
            // 
            contextMenuStripVideoItem.ImageScalingSize = new Size(20, 20);
            contextMenuStripVideoItem.Items.AddRange(new ToolStripItem[] { openFolderToolStripMenuItem });
            contextMenuStripVideoItem.Name = "contextMenuStripVideoItem";
            contextMenuStripVideoItem.Size = new Size(161, 28);
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new Size(160, 24);
            openFolderToolStripMenuItem.Text = "Open Folder";
            // 
            // VideoSourceControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox3);
            Name = "VideoSourceControl";
            Size = new Size(335, 623);
            groupBox3.ResumeLayout(false);
            contextMenuStripVideoItem.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox3;
        private Button button_select_all_video_list;
        private Button button_delete_select_video_list;
        private Button button_clean_video_list;
        private ListBox listBox_video_source;
        private Button button_import_folder;
        private Button button_import_file;
        private Button button_trim_cut;
        private ContextMenuStrip contextMenuStripVideoItem;
        private ToolStripMenuItem openFolderToolStripMenuItem;
    }
}
