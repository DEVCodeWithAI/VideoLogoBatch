namespace VideoLogoBatch
{
    partial class LogoOptionsControl
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
            groupBox1 = new GroupBox();
            checkBox_random_position = new CheckBox();
            textBox_logo_opacity = new TextBox();
            textBox_logosize = new TextBox();
            comboBox_logo_position = new ComboBox();
            textBox_logo_margin = new TextBox();
            label_logo_margin = new Label();
            btn_clean = new Button();
            pic_box_preview_logo = new PictureBox();
            btn_import = new Button();
            trackBar_logo_opacity = new TrackBar();
            label_opacity = new Label();
            trackBar_logosize = new TrackBar();
            label_logosize = new Label();
            label_logo_position = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_box_preview_logo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_logo_opacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_logosize).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox_random_position);
            groupBox1.Controls.Add(textBox_logo_opacity);
            groupBox1.Controls.Add(textBox_logosize);
            groupBox1.Controls.Add(comboBox_logo_position);
            groupBox1.Controls.Add(textBox_logo_margin);
            groupBox1.Controls.Add(label_logo_margin);
            groupBox1.Controls.Add(btn_clean);
            groupBox1.Controls.Add(pic_box_preview_logo);
            groupBox1.Controls.Add(btn_import);
            groupBox1.Controls.Add(trackBar_logo_opacity);
            groupBox1.Controls.Add(label_opacity);
            groupBox1.Controls.Add(trackBar_logosize);
            groupBox1.Controls.Add(label_logosize);
            groupBox1.Controls.Add(label_logo_position);
            groupBox1.Font = new Font("Segoe UI", 10F);
            groupBox1.Location = new Point(2, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(892, 212);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Logo Options";
            // 
            // checkBox_random_position
            // 
            checkBox_random_position.AutoSize = true;
            checkBox_random_position.Location = new Point(335, 168);
            checkBox_random_position.Name = "checkBox_random_position";
            checkBox_random_position.Size = new Size(217, 27);
            checkBox_random_position.TabIndex = 13;
            checkBox_random_position.Text = "Enable Random Position";
            checkBox_random_position.UseVisualStyleBackColor = true;
            // 
            // textBox_logo_opacity
            // 
            textBox_logo_opacity.Location = new Point(755, 98);
            textBox_logo_opacity.Name = "textBox_logo_opacity";
            textBox_logo_opacity.Size = new Size(125, 30);
            textBox_logo_opacity.TabIndex = 12;
            // 
            // textBox_logosize
            // 
            textBox_logosize.Location = new Point(755, 36);
            textBox_logosize.Name = "textBox_logosize";
            textBox_logosize.Size = new Size(125, 30);
            textBox_logosize.TabIndex = 11;
            // 
            // comboBox_logo_position
            // 
            comboBox_logo_position.FormattingEnabled = true;
            comboBox_logo_position.Items.AddRange(new object[] { "Top Center", "Top Left", "Top Right", "Bottom Center", "Bottom Left", "Bottom Right", "Center" });
            comboBox_logo_position.Location = new Point(151, 160);
            comboBox_logo_position.Name = "comboBox_logo_position";
            comboBox_logo_position.Size = new Size(161, 31);
            comboBox_logo_position.TabIndex = 10;
            // 
            // textBox_logo_margin
            // 
            textBox_logo_margin.Location = new Point(709, 167);
            textBox_logo_margin.Name = "textBox_logo_margin";
            textBox_logo_margin.Size = new Size(171, 30);
            textBox_logo_margin.TabIndex = 9;
            textBox_logo_margin.Text = "10";
            textBox_logo_margin.TextAlign = HorizontalAlignment.Center;
            // 
            // label_logo_margin
            // 
            label_logo_margin.AutoSize = true;
            label_logo_margin.Location = new Point(584, 169);
            label_logo_margin.Name = "label_logo_margin";
            label_logo_margin.Size = new Size(111, 23);
            label_logo_margin.TabIndex = 8;
            label_logo_margin.Text = "Logo Margin:";
            // 
            // btn_clean
            // 
            btn_clean.Location = new Point(18, 88);
            btn_clean.Name = "btn_clean";
            btn_clean.Size = new Size(115, 40);
            btn_clean.TabIndex = 7;
            btn_clean.Text = "Delete";
            btn_clean.UseVisualStyleBackColor = true;
            // 
            // pic_box_preview_logo
            // 
            pic_box_preview_logo.Location = new Point(151, 26);
            pic_box_preview_logo.Name = "pic_box_preview_logo";
            pic_box_preview_logo.Size = new Size(161, 110);
            pic_box_preview_logo.TabIndex = 1;
            pic_box_preview_logo.TabStop = false;
            // 
            // btn_import
            // 
            btn_import.Location = new Point(18, 36);
            btn_import.Name = "btn_import";
            btn_import.Size = new Size(115, 40);
            btn_import.TabIndex = 6;
            btn_import.Text = "Import";
            btn_import.UseVisualStyleBackColor = true;
            // 
            // trackBar_logo_opacity
            // 
            trackBar_logo_opacity.Location = new Point(460, 96);
            trackBar_logo_opacity.Name = "trackBar_logo_opacity";
            trackBar_logo_opacity.Size = new Size(289, 56);
            trackBar_logo_opacity.TabIndex = 5;
            // 
            // label_opacity
            // 
            label_opacity.AutoSize = true;
            label_opacity.Location = new Point(335, 96);
            label_opacity.Name = "label_opacity";
            label_opacity.Size = new Size(115, 23);
            label_opacity.TabIndex = 4;
            label_opacity.Text = "Logo Opacity:";
            // 
            // trackBar_logosize
            // 
            trackBar_logosize.Location = new Point(462, 36);
            trackBar_logosize.Name = "trackBar_logosize";
            trackBar_logosize.Size = new Size(289, 56);
            trackBar_logosize.TabIndex = 3;
            // 
            // label_logosize
            // 
            label_logosize.AutoSize = true;
            label_logosize.Location = new Point(335, 36);
            label_logosize.Name = "label_logosize";
            label_logosize.Size = new Size(87, 23);
            label_logosize.TabIndex = 2;
            label_logosize.Text = "Logo Size:";
            // 
            // label_logo_position
            // 
            label_logo_position.AutoSize = true;
            label_logo_position.Font = new Font("Segoe UI", 10F);
            label_logo_position.Location = new Point(18, 163);
            label_logo_position.Name = "label_logo_position";
            label_logo_position.Size = new Size(117, 23);
            label_logo_position.TabIndex = 0;
            label_logo_position.Text = "Logo Position:";
            // 
            // LogoOptionsControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Name = "LogoOptionsControl";
            Size = new Size(897, 215);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_box_preview_logo).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_logo_opacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_logosize).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private CheckBox checkBox_random_position;
        private TextBox textBox_logo_opacity;
        private TextBox textBox_logosize;
        private ComboBox comboBox_logo_position;
        private TextBox textBox_logo_margin;
        private Label label_logo_margin;
        private Button btn_clean;
        private PictureBox pic_box_preview_logo;
        private Button btn_import;
        private TrackBar trackBar_logo_opacity;
        private Label label_opacity;
        private TrackBar trackBar_logosize;
        private Label label_logosize;
        private Label label_logo_position;
    }
}
