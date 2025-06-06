namespace VideoLogoBatch
{
    partial class RenderControl
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
            groupBox4 = new GroupBox();
            label_processing_render = new Label();
            label_status_render = new Label();
            button_openlog = new Button();
            button_stop_render = new Button();
            button_start_render = new Button();
            button_output_folder = new Button();
            label_output_folder = new Label();
            textBox_output_folder = new TextBox();
            comboBox_format = new ComboBox();
            label_format = new Label();
            comboBox__encoder = new ComboBox();
            label_encoder = new Label();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label_processing_render);
            groupBox4.Controls.Add(label_status_render);
            groupBox4.Controls.Add(button_openlog);
            groupBox4.Controls.Add(button_stop_render);
            groupBox4.Controls.Add(button_start_render);
            groupBox4.Controls.Add(button_output_folder);
            groupBox4.Controls.Add(label_output_folder);
            groupBox4.Controls.Add(textBox_output_folder);
            groupBox4.Controls.Add(comboBox_format);
            groupBox4.Controls.Add(label_format);
            groupBox4.Controls.Add(comboBox__encoder);
            groupBox4.Controls.Add(label_encoder);
            groupBox4.Location = new Point(2, 2);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(236, 384);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Render Controls";
            // 
            // label_processing_render
            // 
            label_processing_render.AutoSize = true;
            label_processing_render.Font = new Font("Segoe UI", 10F);
            label_processing_render.Location = new Point(53, 280);
            label_processing_render.Name = "label_processing_render";
            label_processing_render.Size = new Size(131, 23);
            label_processing_render.TabIndex = 19;
            label_processing_render.Text = "Processing... 0%";
            // 
            // label_status_render
            // 
            label_status_render.AutoSize = true;
            label_status_render.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_status_render.Location = new Point(83, 238);
            label_status_render.Name = "label_status_render";
            label_status_render.Size = new Size(65, 23);
            label_status_render.TabIndex = 18;
            label_status_render.Text = "Status:";
            // 
            // button_openlog
            // 
            button_openlog.Location = new Point(6, 342);
            button_openlog.Name = "button_openlog";
            button_openlog.Size = new Size(224, 29);
            button_openlog.TabIndex = 17;
            button_openlog.Text = "Open Log";
            button_openlog.UseVisualStyleBackColor = true;
            // 
            // button_stop_render
            // 
            button_stop_render.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_stop_render.ForeColor = Color.Red;
            button_stop_render.Location = new Point(130, 179);
            button_stop_render.Name = "button_stop_render";
            button_stop_render.Size = new Size(100, 40);
            button_stop_render.TabIndex = 16;
            button_stop_render.Text = "⛔ Stop";
            button_stop_render.UseVisualStyleBackColor = true;
            // 
            // button_start_render
            // 
            button_start_render.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_start_render.ForeColor = Color.LimeGreen;
            button_start_render.Location = new Point(6, 179);
            button_start_render.Name = "button_start_render";
            button_start_render.Size = new Size(100, 40);
            button_start_render.TabIndex = 14;
            button_start_render.Text = "\U0001f7e2 Start";
            button_start_render.UseVisualStyleBackColor = true;
            // 
            // button_output_folder
            // 
            button_output_folder.Location = new Point(6, 132);
            button_output_folder.Name = "button_output_folder";
            button_output_folder.Size = new Size(87, 29);
            button_output_folder.TabIndex = 5;
            button_output_folder.Text = "Browse…";
            button_output_folder.UseVisualStyleBackColor = true;
            // 
            // label_output_folder
            // 
            label_output_folder.AutoSize = true;
            label_output_folder.Font = new Font("Segoe UI", 9F);
            label_output_folder.Location = new Point(6, 109);
            label_output_folder.Name = "label_output_folder";
            label_output_folder.Size = new Size(104, 20);
            label_output_folder.TabIndex = 15;
            label_output_folder.Text = "Output Folder:";
            // 
            // textBox_output_folder
            // 
            textBox_output_folder.Location = new Point(99, 132);
            textBox_output_folder.Name = "textBox_output_folder";
            textBox_output_folder.ReadOnly = true;
            textBox_output_folder.Size = new Size(125, 27);
            textBox_output_folder.TabIndex = 14;
            // 
            // comboBox_format
            // 
            comboBox_format.FormattingEnabled = true;
            comboBox_format.Items.AddRange(new object[] { "MP4 (.mp4)", "MKV (.mkv)", "MOV (.mov)", "AVI (.avi)", "WEBM (.webm)", "FLV (.flv)", "MPEG-TS (.ts)", "WMV (.wmv)" });
            comboBox_format.Location = new Point(85, 67);
            comboBox_format.Name = "comboBox_format";
            comboBox_format.Size = new Size(139, 28);
            comboBox_format.TabIndex = 3;
            // 
            // label_format
            // 
            label_format.AutoSize = true;
            label_format.Font = new Font("Segoe UI", 9F);
            label_format.Location = new Point(6, 70);
            label_format.Name = "label_format";
            label_format.Size = new Size(59, 20);
            label_format.TabIndex = 2;
            label_format.Text = "Format:";
            // 
            // comboBox__encoder
            // 
            comboBox__encoder.FormattingEnabled = true;
            comboBox__encoder.Items.AddRange(new object[] { "CPU (libx264)", "NVIDIA (NVENC)", "AMD (AMF)" });
            comboBox__encoder.Location = new Point(85, 24);
            comboBox__encoder.Name = "comboBox__encoder";
            comboBox__encoder.Size = new Size(139, 28);
            comboBox__encoder.TabIndex = 1;
            // 
            // label_encoder
            // 
            label_encoder.AutoSize = true;
            label_encoder.Font = new Font("Segoe UI", 9F);
            label_encoder.Location = new Point(6, 27);
            label_encoder.Name = "label_encoder";
            label_encoder.Size = new Size(66, 20);
            label_encoder.TabIndex = 0;
            label_encoder.Text = "Encoder:";
            // 
            // RenderControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox4);
            Name = "RenderControl";
            Size = new Size(241, 389);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox4;
        private Label label_processing_render;
        private Label label_status_render;
        private Button button_openlog;
        private Button button_stop_render;
        private Button button_start_render;
        private Button button_output_folder;
        private Label label_output_folder;
        private TextBox textBox_output_folder;
        private ComboBox comboBox_format;
        private Label label_format;
        private ComboBox comboBox__encoder;
        private Label label_encoder;
    }
}
