using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace VideoLogoBatch
{
    public partial class PreviewControl : UserControl
    {
        private const string FfmpegRelativePath = @"ffmpeg\bin\ffmpeg.exe";
        private const string FfprobeRelativePath = @"ffmpeg\bin\ffprobe.exe";
        private string? _currentVideoPathForPreview = null;
        private Image? _originalFrame = null;
        private Image? _currentLogoImage = null;
        private LogoOptionsControl.LogoSettingsData? _currentLogoSettings = null;

        public PreviewControl()
        {
            InitializeComponent();
            pic_box_preview_render.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public bool IsVideoPreviewLoaded()
        {
            return _originalFrame != null;
        }

        private async Task<(int ExitCode, string StandardOutput, string StandardError)> RunProcessAsync(
            string executableFullPath, string? workingDirectory, string arguments, int timeoutMilliseconds = 10000)
        {
            if (string.IsNullOrEmpty(executableFullPath) || !File.Exists(executableFullPath))
            {
                throw new FileNotFoundException($"Executable file not found: {executableFullPath}");
            }
            if (workingDirectory != null && !Directory.Exists(workingDirectory))
            {
                workingDirectory = null;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executableFullPath,
                Arguments = arguments,
                WorkingDirectory = workingDirectory ?? string.Empty,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
            {
                StringBuilder outputBuilder = new StringBuilder();
                StringBuilder errorBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Run(() => {
                    if (!process.WaitForExit(timeoutMilliseconds))
                    {
                        try { if (!process.HasExited) process.Kill(); } catch { /* Ignore errors on kill */ }
                        throw new TimeoutException($"Process {Path.GetFileName(executableFullPath)} timed out after {timeoutMilliseconds / 1000} seconds.");
                    }
                });

                await Task.Delay(100);

                return (process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
            }
        }
        private async Task<double?> GetVideoDurationAsync(string videoPath, string ffprobeFullPath, string? ffprobeDirectory)
        {
            string arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{videoPath}\"";
            try
            {
                var result = await RunProcessAsync(ffprobeFullPath, ffprobeDirectory, arguments, 5000);

                if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.StandardOutput))
                {
                    if (double.TryParse(result.StandardOutput.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double duration))
                    {
                        return duration;
                    }
                    else
                    {
                        Debug.WriteLine($"Could not parse duration from ffprobe output: {result.StandardOutput}");
                    }
                }
                else
                {
                    Debug.WriteLine($"ffprobe error or no output. ExitCode: {result.ExitCode}, Error: {result.StandardError}");
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "ffprobe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TimeoutException ex)
            {
                Debug.WriteLine($"ffprobe timed out: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error while getting video duration: {ex.Message}");
            }
            return null;
        }
        private string FormatTimeSpanForFFmpeg(TimeSpan ts)
        {
            return ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }

        public async Task LoadVideoFrameAsync(string? videoPath)
        {
            ClearPreviewResources();

            _currentVideoPathForPreview = videoPath;

            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
            {
                if (pic_box_preview_render.Image != null)
                {
                    pic_box_preview_render.Image.Dispose();
                    pic_box_preview_render.Image = null;
                }
                return;
            }

            string tempFrameFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".png");
            string outputFramePath = Path.Combine(Path.GetTempPath(), tempFrameFileName);

            string ffmpegFullPath = Path.Combine(Application.StartupPath, FfmpegRelativePath);
            string? ffmpegDirectory = Path.GetDirectoryName(ffmpegFullPath);
            string ffprobeFullPath = Path.Combine(Application.StartupPath, FfprobeRelativePath);
            string? ffprobeDirectory = Path.GetDirectoryName(ffprobeFullPath);

            if (!File.Exists(ffmpegFullPath))
            {
                MessageBox.Show($"FFmpeg not found at: '{ffmpegFullPath}'.", "FFmpeg Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(ffprobeFullPath))
            {
                MessageBox.Show($"ffprobe not found at: '{ffprobeFullPath}'.", "ffprobe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(ffmpegDirectory) || string.IsNullOrEmpty(ffprobeDirectory))
            {
                MessageBox.Show("Could not determine the working directory for FFmpeg/ffprobe.", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string seekArgument = "-ss 00:00:01";

            try
            {
                double? durationSeconds = await GetVideoDurationAsync(videoPath, ffprobeFullPath, ffprobeDirectory);

                if (durationSeconds.HasValue && durationSeconds.Value > 0)
                {
                    double targetSeekSeconds = durationSeconds.Value * 0.30;
                    if (targetSeekSeconds >= durationSeconds.Value && durationSeconds.Value > 0.1)
                    {
                        targetSeekSeconds = durationSeconds.Value * 0.1;
                    }
                    if (targetSeekSeconds < 0.1 && durationSeconds.Value > 0.2)
                    {
                        targetSeekSeconds = 0.1;
                    }
                    else if (targetSeekSeconds < 0)
                    {
                        targetSeekSeconds = 0;
                    }
                    seekArgument = $"-ss {FormatTimeSpanForFFmpeg(TimeSpan.FromSeconds(targetSeekSeconds))}";
                }

                string ffmpegArguments = $"-y {seekArgument} -i \"{videoPath}\" -vframes 1 -an \"{outputFramePath}\"";
                var ffmpegResult = await RunProcessAsync(ffmpegFullPath, ffmpegDirectory, ffmpegArguments, 15000);

                if (ffmpegResult.ExitCode == 0 && File.Exists(outputFramePath))
                {
                    using (FileStream fs = new FileStream(outputFramePath, FileMode.Open, FileAccess.Read, FileShare.Read)) // Add FileShare.Read
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await fs.CopyToAsync(ms);
                            ms.Position = 0;

                            _originalFrame?.Dispose();

                            using (Image rawFrame = Image.FromStream(ms))
                            {
                                int targetPreviewWidth = pic_box_preview_render.ClientRectangle.Width;
                                int targetPreviewHeight = pic_box_preview_render.ClientRectangle.Height;

                                if (targetPreviewWidth <= 0) targetPreviewWidth = 640;
                                if (targetPreviewHeight <= 0) targetPreviewHeight = 360;

                                if (rawFrame.Width > targetPreviewWidth || rawFrame.Height > targetPreviewHeight)
                                {
                                    float ratioX = (float)targetPreviewWidth / rawFrame.Width;
                                    float ratioY = (float)targetPreviewHeight / rawFrame.Height;
                                    float ratio = Math.Min(ratioX, ratioY);
                                    int newWidth = (int)(rawFrame.Width * ratio);
                                    int newHeight = (int)(rawFrame.Height * ratio);
                                    if (newWidth < 1) newWidth = 1;
                                    if (newHeight < 1) newHeight = 1;

                                    PixelFormat originalPixelFormat = rawFrame.PixelFormat;
                                    if (originalPixelFormat == PixelFormat.Format1bppIndexed ||
                                        originalPixelFormat == PixelFormat.Format4bppIndexed ||
                                        originalPixelFormat == PixelFormat.Format8bppIndexed ||
                                        (rawFrame.Flags & (int)ImageFlags.HasAlpha) == 0)
                                    {
                                        originalPixelFormat = PixelFormat.Format32bppArgb;
                                    }

                                    Bitmap scaledBitmap = new Bitmap(newWidth, newHeight, originalPixelFormat);
                                    scaledBitmap.SetResolution(rawFrame.HorizontalResolution, rawFrame.VerticalResolution);
                                    using (Graphics g = Graphics.FromImage(scaledBitmap))
                                    {
                                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        g.DrawImage(rawFrame, 0, 0, newWidth, newHeight);
                                    }
                                    _originalFrame = scaledBitmap;
                                }
                                else
                                {
                                    _originalFrame = new Bitmap(rawFrame);
                                }
                            }
                        }
                    }
                    RenderCurrentPreview();
                }
                else
                {
                    _originalFrame?.Dispose();
                    _originalFrame = null;
                    RenderCurrentPreview();
                    MessageBox.Show($"Error extracting frame.\nFFmpeg Exit Code: {ffmpegResult.ExitCode}\nError Details:\n{ffmpegResult.StandardError}", "FFmpeg Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.Message, "Timeout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while creating the preview: {ex.Message}", "General Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (File.Exists(outputFramePath))
                {
                    TryDeleteFile(outputFramePath);
                }
            }
        }

        public void UpdateLogoOnPreview(LogoOptionsControl.LogoSettingsData? logoSettings)
        {
            _currentLogoSettings = logoSettings;

            _currentLogoImage?.Dispose();
            _currentLogoImage = null;

            if (_currentLogoSettings != null && !string.IsNullOrEmpty(_currentLogoSettings.Path) && File.Exists(_currentLogoSettings.Path))
            {
                try
                {
                    _currentLogoImage = Image.FromFile(_currentLogoSettings.Path);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading logo image for preview: {ex.Message}");
                    _currentLogoImage = null;
                }
            }
            RenderCurrentPreview();
        }

        private void RenderCurrentPreview()
        {
            if (_originalFrame == null)
            {
                pic_box_preview_render.Image?.Dispose();
                pic_box_preview_render.Image = null;
                return;
            }

            Image frameToDisplay = (Image)_originalFrame.Clone();

            if (_currentLogoImage != null && _currentLogoSettings != null)
            {
                if (_currentLogoSettings.IsRandomPosition)
                {
                    using (Graphics g = Graphics.FromImage(frameToDisplay))
                    {
                        string message = "Moving Logo Effect";
                        Font font = new Font("Arial", 16, FontStyle.Bold);
                        SizeF textSize = g.MeasureString(message, font);
                        PointF location = new PointF(
                            (frameToDisplay.Width - textSize.Width) / 2,
                            (frameToDisplay.Height - textSize.Height) / 2);
                        g.DrawString(message, font, Brushes.Yellow, location);
                        font.Dispose();
                    }
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(frameToDisplay))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        float logoAspectRatio = (float)_currentLogoImage.Width / _currentLogoImage.Height;
                        int logoNewWidth = (int)(_originalFrame.Width * (_currentLogoSettings.SizePercent / 100f));
                        int logoNewHeight = (int)(logoNewWidth / logoAspectRatio);

                        if (logoNewHeight > _originalFrame.Height * 0.95)
                        {
                            logoNewHeight = (int)(_originalFrame.Height * 0.95);
                            logoNewWidth = (int)(logoNewHeight * logoAspectRatio);
                        }
                        if (logoNewWidth > _originalFrame.Width * 0.95)
                        {
                            logoNewWidth = (int)(_originalFrame.Width * 0.95);
                            logoNewHeight = (int)(logoNewWidth / logoAspectRatio);
                        }
                        if (logoNewWidth > 0 && logoNewHeight > 0)
                        {
                            Point logoPosition = CalculateLogoPosition(
                                _originalFrame.Width, _originalFrame.Height,
                                logoNewWidth, logoNewHeight,
                                _currentLogoSettings.Position, _currentLogoSettings.Margin
                            );

                            ImageAttributes imageAttributes = new ImageAttributes();
                            if (_currentLogoSettings.OpacityPercent < 100)
                            {
                                ColorMatrix colorMatrix = new ColorMatrix();
                                colorMatrix.Matrix33 = _currentLogoSettings.OpacityPercent / 100f;
                                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            }

                            g.DrawImage(
                                _currentLogoImage,
                                new Rectangle(logoPosition.X, logoPosition.Y, logoNewWidth, logoNewHeight),
                                0, 0, _currentLogoImage.Width, _currentLogoImage.Height,
                                GraphicsUnit.Pixel,
                                imageAttributes
                            );
                        }
                    }
                }
            }

            pic_box_preview_render.Image?.Dispose();
            pic_box_preview_render.Image = frameToDisplay;
        }

        private Point CalculateLogoPosition(int frameWidth, int frameHeight, int logoWidth, int logoHeight, string positionName, int margin)
        {
            int x = margin;
            int y = margin;

            switch (positionName)
            {
                case "Top Left":
                    x = margin;
                    y = margin;
                    break;
                case "Top Center":
                    x = (frameWidth - logoWidth) / 2;
                    y = margin;
                    break;
                case "Top Right":
                    x = frameWidth - logoWidth - margin;
                    y = margin;
                    break;
                case "Bottom Left":
                    x = margin;
                    y = frameHeight - logoHeight - margin;
                    break;
                case "Bottom Center":
                    x = (frameWidth - logoWidth) / 2;
                    y = frameHeight - logoHeight - margin;
                    break;
                case "Bottom Right":
                    x = frameWidth - logoWidth - margin;
                    y = frameHeight - logoHeight - margin;
                    break;
                case "Center":
                    x = (frameWidth - logoWidth) / 2;
                    y = (frameHeight - logoHeight) / 2;
                    break;
                default:
                    x = margin;
                    y = margin;
                    break;
            }
            x = Math.Max(0, Math.Min(x, frameWidth - logoWidth));
            y = Math.Max(0, Math.Min(y, frameHeight - logoHeight));

            return new Point(x, y);
        }


        public void ClearPreviewResources()
        {
            pic_box_preview_render.Image?.Dispose();
            pic_box_preview_render.Image = null;

            _originalFrame?.Dispose();
            _originalFrame = null;

            _currentLogoImage?.Dispose();
            _currentLogoImage = null;

            _currentVideoPathForPreview = null;
            _currentLogoSettings = null;
        }

        private void TryDeleteFile(string filePath)
        {
            try { File.Delete(filePath); }
            catch (IOException ex) { Debug.WriteLine($"Could not delete temporary file: {filePath}. Error: {ex.Message}"); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPreviewResources();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}