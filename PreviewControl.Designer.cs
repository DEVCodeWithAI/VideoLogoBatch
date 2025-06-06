namespace VideoLogoBatch
{
    partial class PreviewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox2 = new GroupBox();
            pic_box_preview_render = new PictureBox();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_box_preview_render).BeginInit();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(pic_box_preview_render);
            groupBox2.Location = new Point(2, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(660, 400);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Preview";
            // 
            // pic_box_preview_render
            // 
            pic_box_preview_render.Location = new Point(10, 27);
            pic_box_preview_render.Name = "pic_box_preview_render";
            pic_box_preview_render.Size = new Size(640, 360);
            pic_box_preview_render.TabIndex = 0;
            pic_box_preview_render.TabStop = false;
            // 
            // PreviewControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox2);
            Name = "PreviewControl";
            Size = new Size(665, 405);
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pic_box_preview_render).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox2;
        private PictureBox pic_box_preview_render;
    }
}
