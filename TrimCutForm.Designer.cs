namespace VideoLogoBatch
{
    partial class TrimCutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrimCutForm));
            groupBox_preview_video = new GroupBox();
            pictureBox_video_preview = new PictureBox();
            groupBox_panel = new GroupBox();
            button_cancel_cut = new Button();
            button_save_cut = new Button();
            label_end_time = new Label();
            label_start_time = new Label();
            textBox_end_time = new TextBox();
            textBox_start_time = new TextBox();
            button_set_end = new Button();
            button_set_start = new Button();
            label_time_ms = new Label();
            label_time_ss = new Label();
            label_time_mm = new Label();
            label_time_hh = new Label();
            textBox_time_ms = new TextBox();
            textBox_time_ss = new TextBox();
            textBox_time_mm = new TextBox();
            textBox_time_hh = new TextBox();
            label_time_line = new Label();
            trackBar_time_line = new TrackBar();
            groupBox_preview_video.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_video_preview).BeginInit();
            groupBox_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_time_line).BeginInit();
            SuspendLayout();
            // 
            // groupBox_preview_video
            // 
            groupBox_preview_video.Controls.Add(pictureBox_video_preview);
            groupBox_preview_video.Location = new Point(12, 12);
            groupBox_preview_video.Name = "groupBox_preview_video";
            groupBox_preview_video.Size = new Size(1300, 760);
            groupBox_preview_video.TabIndex = 0;
            groupBox_preview_video.TabStop = false;
            groupBox_preview_video.Text = "Preview Video";
            // 
            // pictureBox_video_preview
            // 
            pictureBox_video_preview.Location = new Point(11, 26);
            pictureBox_video_preview.Name = "pictureBox_video_preview";
            pictureBox_video_preview.Size = new Size(1280, 720);
            pictureBox_video_preview.TabIndex = 0;
            pictureBox_video_preview.TabStop = false;
            // 
            // groupBox_panel
            // 
            groupBox_panel.Controls.Add(button_cancel_cut);
            groupBox_panel.Controls.Add(button_save_cut);
            groupBox_panel.Controls.Add(label_end_time);
            groupBox_panel.Controls.Add(label_start_time);
            groupBox_panel.Controls.Add(textBox_end_time);
            groupBox_panel.Controls.Add(textBox_start_time);
            groupBox_panel.Controls.Add(button_set_end);
            groupBox_panel.Controls.Add(button_set_start);
            groupBox_panel.Controls.Add(label_time_ms);
            groupBox_panel.Controls.Add(label_time_ss);
            groupBox_panel.Controls.Add(label_time_mm);
            groupBox_panel.Controls.Add(label_time_hh);
            groupBox_panel.Controls.Add(textBox_time_ms);
            groupBox_panel.Controls.Add(textBox_time_ss);
            groupBox_panel.Controls.Add(textBox_time_mm);
            groupBox_panel.Controls.Add(textBox_time_hh);
            groupBox_panel.Controls.Add(label_time_line);
            groupBox_panel.Controls.Add(trackBar_time_line);
            groupBox_panel.Font = new Font("Segoe UI", 9F);
            groupBox_panel.Location = new Point(12, 778);
            groupBox_panel.Name = "groupBox_panel";
            groupBox_panel.Size = new Size(1300, 203);
            groupBox_panel.TabIndex = 2;
            groupBox_panel.TabStop = false;
            groupBox_panel.Text = "Panel";
            // 
            // button_cancel_cut
            // 
            button_cancel_cut.BackColor = Color.Khaki;
            button_cancel_cut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_cancel_cut.Location = new Point(156, 159);
            button_cancel_cut.Name = "button_cancel_cut";
            button_cancel_cut.Size = new Size(125, 35);
            button_cancel_cut.TabIndex = 17;
            button_cancel_cut.Text = "Cancel Cut";
            button_cancel_cut.UseVisualStyleBackColor = false;
            button_cancel_cut.Click += button_cancel_trim_Click;
            // 
            // button_save_cut
            // 
            button_save_cut.BackColor = Color.YellowGreen;
            button_save_cut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_save_cut.Location = new Point(11, 159);
            button_save_cut.Name = "button_save_cut";
            button_save_cut.Size = new Size(125, 35);
            button_save_cut.TabIndex = 16;
            button_save_cut.Text = "Save Cut";
            button_save_cut.UseVisualStyleBackColor = false;
            // 
            // label_end_time
            // 
            label_end_time.AutoSize = true;
            label_end_time.BackColor = SystemColors.Control;
            label_end_time.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_end_time.ForeColor = Color.Crimson;
            label_end_time.Location = new Point(38, 96);
            label_end_time.Name = "label_end_time";
            label_end_time.Size = new Size(74, 20);
            label_end_time.TabIndex = 15;
            label_end_time.Text = "End Time";
            // 
            // label_start_time
            // 
            label_start_time.AutoSize = true;
            label_start_time.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_start_time.ForeColor = SystemColors.Highlight;
            label_start_time.Location = new Point(35, 36);
            label_start_time.Name = "label_start_time";
            label_start_time.Size = new Size(82, 20);
            label_start_time.TabIndex = 14;
            label_start_time.Text = "Start Time";
            // 
            // textBox_end_time
            // 
            textBox_end_time.Location = new Point(11, 119);
            textBox_end_time.Name = "textBox_end_time";
            textBox_end_time.Size = new Size(125, 27);
            textBox_end_time.TabIndex = 13;
            // 
            // textBox_start_time
            // 
            textBox_start_time.Location = new Point(11, 59);
            textBox_start_time.Name = "textBox_start_time";
            textBox_start_time.Size = new Size(125, 27);
            textBox_start_time.TabIndex = 12;
            // 
            // button_set_end
            // 
            button_set_end.BackColor = Color.Crimson;
            button_set_end.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_set_end.ForeColor = SystemColors.ButtonHighlight;
            button_set_end.Location = new Point(756, 159);
            button_set_end.Name = "button_set_end";
            button_set_end.Size = new Size(95, 35);
            button_set_end.TabIndex = 11;
            button_set_end.Text = "Set End";
            button_set_end.UseVisualStyleBackColor = false;
            // 
            // button_set_start
            // 
            button_set_start.BackColor = SystemColors.Highlight;
            button_set_start.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_set_start.ForeColor = SystemColors.ButtonHighlight;
            button_set_start.Location = new Point(613, 159);
            button_set_start.Name = "button_set_start";
            button_set_start.Size = new Size(95, 35);
            button_set_start.TabIndex = 10;
            button_set_start.Text = "Set Start";
            button_set_start.UseVisualStyleBackColor = false;
            // 
            // label_time_ms
            // 
            label_time_ms.AutoSize = true;
            label_time_ms.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_time_ms.Location = new Point(807, 26);
            label_time_ms.Name = "label_time_ms";
            label_time_ms.Size = new Size(30, 20);
            label_time_ms.TabIndex = 9;
            label_time_ms.Text = "ms";
            // 
            // label_time_ss
            // 
            label_time_ss.AutoSize = true;
            label_time_ss.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_time_ss.Location = new Point(750, 26);
            label_time_ss.Name = "label_time_ss";
            label_time_ss.Size = new Size(25, 20);
            label_time_ss.TabIndex = 8;
            label_time_ss.Text = "SS";
            // 
            // label_time_mm
            // 
            label_time_mm.AutoSize = true;
            label_time_mm.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_time_mm.Location = new Point(684, 26);
            label_time_mm.Name = "label_time_mm";
            label_time_mm.Size = new Size(37, 20);
            label_time_mm.TabIndex = 7;
            label_time_mm.Text = "MM";
            // 
            // label_time_hh
            // 
            label_time_hh.AutoSize = true;
            label_time_hh.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label_time_hh.Location = new Point(626, 26);
            label_time_hh.Name = "label_time_hh";
            label_time_hh.Size = new Size(31, 20);
            label_time_hh.TabIndex = 6;
            label_time_hh.Text = "HH";
            // 
            // textBox_time_ms
            // 
            textBox_time_ms.Location = new Point(796, 49);
            textBox_time_ms.Name = "textBox_time_ms";
            textBox_time_ms.Size = new Size(55, 27);
            textBox_time_ms.TabIndex = 5;
            // 
            // textBox_time_ss
            // 
            textBox_time_ss.Location = new Point(735, 49);
            textBox_time_ss.Name = "textBox_time_ss";
            textBox_time_ss.Size = new Size(55, 27);
            textBox_time_ss.TabIndex = 4;
            // 
            // textBox_time_mm
            // 
            textBox_time_mm.Location = new Point(674, 49);
            textBox_time_mm.Name = "textBox_time_mm";
            textBox_time_mm.Size = new Size(55, 27);
            textBox_time_mm.TabIndex = 3;
            // 
            // textBox_time_hh
            // 
            textBox_time_hh.Location = new Point(613, 49);
            textBox_time_hh.Name = "textBox_time_hh";
            textBox_time_hh.Size = new Size(55, 27);
            textBox_time_hh.TabIndex = 2;
            // 
            // label_time_line
            // 
            label_time_line.AutoSize = true;
            label_time_line.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_time_line.Location = new Point(156, 49);
            label_time_line.Name = "label_time_line";
            label_time_line.Size = new Size(88, 23);
            label_time_line.TabIndex = 1;
            label_time_line.Text = "Time Line";
            // 
            // trackBar_time_line
            // 
            trackBar_time_line.BackColor = Color.SkyBlue;
            trackBar_time_line.Location = new Point(156, 90);
            trackBar_time_line.Name = "trackBar_time_line";
            trackBar_time_line.Size = new Size(1135, 56);
            trackBar_time_line.TabIndex = 0;
            // 
            // TrimCutForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1324, 993);
            Controls.Add(groupBox_panel);
            Controls.Add(groupBox_preview_video);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TrimCutForm";
            Text = "Trim Cut";
            groupBox_preview_video.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox_video_preview).EndInit();
            groupBox_panel.ResumeLayout(false);
            groupBox_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_time_line).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox_preview_video;
        private PictureBox pictureBox_video_preview;
        private GroupBox groupBox_panel;
        private Label label_time_line;
        private TrackBar trackBar_time_line;
        private TextBox textBox_time_ms;
        private TextBox textBox_time_ss;
        private TextBox textBox_time_mm;
        private TextBox textBox_time_hh;
        private Button button_set_end;
        private Button button_set_start;
        private Label label_time_ms;
        private Label label_time_ss;
        private Label label_time_mm;
        private Label label_time_hh;
        private Button button_save_cut;
        private Label label_end_time;
        private Label label_start_time;
        private TextBox textBox_end_time;
        private TextBox textBox_start_time;
        private Button button_cancel_cut;
    }
}