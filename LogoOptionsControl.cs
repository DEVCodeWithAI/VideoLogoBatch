using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace VideoLogoBatch
{
    public partial class LogoOptionsControl : UserControl
    {
        private string? _currentLogoPath = null;
        public event EventHandler? LogoSettingsChanged;

        private int _logoSizePercent = 50;
        private int _logoOpacityPercent = 100;
        private int _logoMargin = 10;

        public LogoOptionsControl()
        {
            InitializeComponent();

            pic_box_preview_logo.SizeMode = PictureBoxSizeMode.Zoom;

            btn_import.Click += Btn_import_Click;
            btn_clean.Click += Btn_clean_Click;
            comboBox_logo_position.SelectedIndexChanged += ComboBox_logo_position_SelectedIndexChanged;
            trackBar_logosize.Scroll += TrackBar_logosize_Scroll;
            textBox_logosize.TextChanged += TextBox_logosize_TextChanged;
            textBox_logosize.KeyPress += TextBox_Numeric_KeyPress;
            trackBar_logo_opacity.Scroll += TrackBar_logo_opacity_Scroll;
            textBox_logo_opacity.TextChanged += TextBox_logo_opacity_TextChanged;
            textBox_logo_opacity.KeyPress += TextBox_Numeric_KeyPress;
            textBox_logo_margin.TextChanged += TextBox_logo_margin_TextChanged;
            textBox_logo_margin.KeyPress += TextBox_Numeric_KeyPress;
            checkBox_random_position.CheckedChanged += CheckBox_random_position_CheckedChanged;

            InitializeDefaultValues();
        }

        private void InitializeDefaultValues()
        {
            if (comboBox_logo_position.Items.Contains("Top Center"))
            {
                comboBox_logo_position.SelectedItem = "Top Center";
            }
            else if (comboBox_logo_position.Items.Count > 0)
            {
                comboBox_logo_position.SelectedIndex = 0;
            }

            trackBar_logosize.Minimum = 1;
            trackBar_logosize.Maximum = 100;
            _logoSizePercent = Math.Clamp(_logoSizePercent, trackBar_logosize.Minimum, trackBar_logosize.Maximum);
            trackBar_logosize.Value = _logoSizePercent;
            textBox_logosize.Text = _logoSizePercent.ToString();

            trackBar_logo_opacity.Minimum = 0;
            trackBar_logo_opacity.Maximum = 100;
            _logoOpacityPercent = Math.Clamp(_logoOpacityPercent, trackBar_logo_opacity.Minimum, trackBar_logo_opacity.Maximum);
            trackBar_logo_opacity.Value = _logoOpacityPercent;
            textBox_logo_opacity.Text = _logoOpacityPercent.ToString();

            textBox_logo_margin.Text = _logoMargin.ToString();
        }

        private Image CreateThumbnail(Image originalImage, int maxWidth, int maxHeight)
        {
            if (originalImage == null) throw new ArgumentNullException(nameof(originalImage));

            if (originalImage.Width <= maxWidth && originalImage.Height <= maxHeight)
            {
                return new Bitmap(originalImage);
            }

            float ratioX = (float)maxWidth / originalImage.Width;
            float ratioY = (float)maxHeight / originalImage.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalImage.Width * ratio);
            int newHeight = (int)(originalImage.Height * ratio);

            if (newWidth <= 0) newWidth = 1;
            if (newHeight <= 0) newHeight = 1;

            PixelFormat originalPixelFormat = originalImage.PixelFormat;
            if (originalPixelFormat == PixelFormat.Format1bppIndexed ||
                originalPixelFormat == PixelFormat.Format4bppIndexed ||
                originalPixelFormat == PixelFormat.Format8bppIndexed ||
                (originalImage.Flags & (int)ImageFlags.HasAlpha) == 0)
            {
                originalPixelFormat = PixelFormat.Format32bppArgb;
            }

            Bitmap newImage = new Bitmap(newWidth, newHeight, originalPixelFormat);
            newImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private void Btn_import_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn ảnh Logo";
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|PNG files (*.png)|*.png|JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    Image? oldImageInPictureBox = pic_box_preview_logo.Image;
                    pic_box_preview_logo.Image = null;

                    try
                    {
                        using (Image originalImage = Image.FromFile(selectedFilePath))
                        {
                            Image thumbnail = CreateThumbnail(originalImage, pic_box_preview_logo.Width, pic_box_preview_logo.Height);
                            pic_box_preview_logo.Image = thumbnail;
                        }
                        _currentLogoPath = selectedFilePath;
                        OnLogoSettingsChanged();
                    }
                    catch (OutOfMemoryException memEx)
                    {
                        MessageBox.Show($"Lỗi hết bộ nhớ khi mở tệp ảnh. Tệp có thể quá lớn hoặc bị lỗi.\nChi tiết: {memEx.Message}", "Lỗi Ảnh Logo - OutOfMemory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        pic_box_preview_logo.Image = oldImageInPictureBox;
                        oldImageInPictureBox = null;
                        _currentLogoPath = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi mở tệp ảnh: {ex.Message}", "Lỗi Ảnh Logo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        pic_box_preview_logo.Image = oldImageInPictureBox;
                        oldImageInPictureBox = null;
                        _currentLogoPath = null;
                    }
                    finally
                    {
                        oldImageInPictureBox?.Dispose();
                    }
                }
            }
        }

        private void Btn_clean_Click(object? sender, EventArgs e)
        {
            pic_box_preview_logo.Image?.Dispose();
            pic_box_preview_logo.Image = null;
            _currentLogoPath = null;
            OnLogoSettingsChanged();
        }

        private void ComboBox_logo_position_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isCenterPosition = comboBox_logo_position.SelectedItem?.ToString() == "Center";
            textBox_logo_margin.Enabled = !isCenterPosition && !checkBox_random_position.Checked;
            OnLogoSettingsChanged();
        }

        private bool _isUpdatingTextBoxSize = false;
        private void TrackBar_logosize_Scroll(object? sender, EventArgs e)
        {
            if (textBox_logosize.Text != trackBar_logosize.Value.ToString())
            {
                _logoSizePercent = trackBar_logosize.Value;
                _isUpdatingTextBoxSize = true;
                textBox_logosize.Text = _logoSizePercent.ToString();
                _isUpdatingTextBoxSize = false;
                OnLogoSettingsChanged();
            }
        }

        private void TextBox_logosize_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingTextBoxSize) return;

            if (int.TryParse(textBox_logosize.Text, out int value))
            {
                value = Math.Clamp(value, trackBar_logosize.Minimum, trackBar_logosize.Maximum);
                if (trackBar_logosize.Value != value)
                {
                    trackBar_logosize.Value = value;
                }
                if (_logoSizePercent != value)
                {
                    _logoSizePercent = value;
                    OnLogoSettingsChanged();
                }
                if (textBox_logosize.Text != value.ToString())
                {
                    _isUpdatingTextBoxSize = true;
                    textBox_logosize.Text = value.ToString();
                    textBox_logosize.SelectionStart = textBox_logosize.Text.Length;
                    _isUpdatingTextBoxSize = false;
                }
            }
        }

        private bool _isUpdatingTextBoxOpacity = false;
        private void TrackBar_logo_opacity_Scroll(object? sender, EventArgs e)
        {
            if (textBox_logo_opacity.Text != trackBar_logo_opacity.Value.ToString())
            {
                _logoOpacityPercent = trackBar_logo_opacity.Value;
                _isUpdatingTextBoxOpacity = true;
                textBox_logo_opacity.Text = _logoOpacityPercent.ToString();
                _isUpdatingTextBoxOpacity = false;
                OnLogoSettingsChanged();
            }
        }

        private void TextBox_logo_opacity_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingTextBoxOpacity) return;

            if (int.TryParse(textBox_logo_opacity.Text, out int value))
            {
                value = Math.Clamp(value, trackBar_logo_opacity.Minimum, trackBar_logo_opacity.Maximum);
                if (trackBar_logo_opacity.Value != value)
                {
                    trackBar_logo_opacity.Value = value;
                }
                if (_logoOpacityPercent != value)
                {
                    _logoOpacityPercent = value;
                    OnLogoSettingsChanged();
                }
                if (textBox_logo_opacity.Text != value.ToString())
                {
                    _isUpdatingTextBoxOpacity = true;
                    textBox_logo_opacity.Text = value.ToString();
                    textBox_logo_opacity.SelectionStart = textBox_logo_opacity.Text.Length;
                    _isUpdatingTextBoxOpacity = false;
                }
            }
        }

        private bool _isUpdatingTextBoxMargin = false;
        private void TextBox_logo_margin_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingTextBoxMargin) return;

            if (int.TryParse(textBox_logo_margin.Text, out int value))
            {
                value = Math.Max(0, value);
                if (_logoMargin != value)
                {
                    _logoMargin = value;
                    OnLogoSettingsChanged();
                }
                if (textBox_logo_margin.Text != value.ToString())
                {
                    _isUpdatingTextBoxMargin = true;
                    textBox_logo_margin.Text = value.ToString();
                    textBox_logo_margin.SelectionStart = textBox_logo_margin.Text.Length;
                    _isUpdatingTextBoxMargin = false;
                }
            }
        }

        private void CheckBox_random_position_CheckedChanged(object? sender, EventArgs e)
        {
            bool isFloatingLogoEnabled = checkBox_random_position.Checked;
            comboBox_logo_position.Enabled = !isFloatingLogoEnabled;

            bool isCenterPositionStatic = comboBox_logo_position.SelectedItem?.ToString() == "Center";
            textBox_logo_margin.Enabled = !isFloatingLogoEnabled || (!isCenterPositionStatic && !isFloatingLogoEnabled);

            OnLogoSettingsChanged();
        }

        private void TextBox_Numeric_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected virtual void OnLogoSettingsChanged()
        {
            LogoSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public string? GetCurrentLogoPath() => _currentLogoPath;

        public string GetLogoPosition() => comboBox_logo_position.SelectedItem?.ToString() ?? (comboBox_logo_position.Items.Count > 0 ? (comboBox_logo_position.Items[0]?.ToString()) ?? "Top Left" : "Top Left");

        public int GetLogoSizePercent() => _logoSizePercent;
        public int GetLogoOpacityPercent() => _logoOpacityPercent;
        public int GetLogoMargin() => _logoMargin;
        public bool IsRandomPositionEnabled() => checkBox_random_position.Checked;

        public class LogoSettingsData
        {
            public string? Path { get; set; }
            public string Position { get; set; } = "Top Center";
            public int SizePercent { get; set; } = 50;
            public int OpacityPercent { get; set; } = 100;
            public int Margin { get; set; } = 10;
            public bool IsRandomPosition { get; set; } = false;
        }

        public LogoSettingsData GetCurrentLogoSettings()
        {
            string position = "Top Left";
            if (comboBox_logo_position.SelectedItem != null)
            {
                position = comboBox_logo_position.SelectedItem.ToString() ?? position;
            }
            else if (comboBox_logo_position.Items.Count > 0)
            {
                position = (comboBox_logo_position.Items[0]?.ToString()) ?? position;
            }

            return new LogoSettingsData
            {
                Path = _currentLogoPath,
                Position = position,
                SizePercent = _logoSizePercent,
                OpacityPercent = _logoOpacityPercent,
                Margin = _logoMargin,
                IsRandomPosition = checkBox_random_position.Checked
            };
        }
    }
}