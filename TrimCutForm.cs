using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoLogoBatch
{
    public partial class TrimCutForm : Form
    {
        private string _currentVideoPath;
        private TimeSpan _videoTotalDuration;
        private TimeSpan _trimStartTime = TimeSpan.Zero;
        private TimeSpan _trimEndTime;

        private Image? _currentFrameBitmap = null;
        private CancellationTokenSource? _frameCaptureCts = null;

        private readonly string _ffmpegExecutablePath;
        private readonly string _ffmpegDirectory;

        private System.Windows.Forms.Timer _previewUpdateTimer;
        private const int PreviewUpdateDelayMs = 300;
        private TimeSpan _lastTimeFromTrackBarOrTextBox;

        private bool _isUpdatingTimeFromTrackBar = false;
        private bool _isUpdatingTimeFromTextBox = false;

        public TimeSpan UserTrimStartTime => _trimStartTime;
        public TimeSpan UserTrimEndTime => _trimEndTime;
        public string? OutputCutFilePath { get; private set; }

        public TrimCutForm(string videoPath, TimeSpan totalDuration)
        {
            InitializeComponent();

            _currentVideoPath = videoPath;
            _videoTotalDuration = totalDuration;
            _trimEndTime = totalDuration;

            string ffmpegRelativePath = @"ffmpeg\bin\ffmpeg.exe";
            _ffmpegExecutablePath = Path.Combine(Application.StartupPath, ffmpegRelativePath);
            _ffmpegDirectory = Path.GetDirectoryName(_ffmpegExecutablePath) ?? Application.StartupPath;

            this.Text = $"Trim/Cut - {Path.GetFileName(_currentVideoPath)}";

            _previewUpdateTimer = new System.Windows.Forms.Timer();
            _previewUpdateTimer.Interval = PreviewUpdateDelayMs;
            _previewUpdateTimer.Tick += PreviewUpdateTimer_Tick;

            this.Load += TrimCutForm_Load;
            this.FormClosed += TrimCutForm_FormClosed;

            if (this.trackBar_time_line != null)
                this.trackBar_time_line.Scroll += new System.EventHandler(this.trackBar_time_line_Scroll);

            if (this.textBox_time_hh != null)
            {
                this.textBox_time_hh.TextChanged += new System.EventHandler(this.TimeTextBox_TextChanged);
                this.textBox_time_hh.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Numeric_KeyPress_Shared);
            }
            if (this.textBox_time_mm != null)
            {
                this.textBox_time_mm.TextChanged += new System.EventHandler(this.TimeTextBox_TextChanged);
                this.textBox_time_mm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Numeric_KeyPress_Shared);
            }
            if (this.textBox_time_ss != null)
            {
                this.textBox_time_ss.TextChanged += new System.EventHandler(this.TimeTextBox_TextChanged);
                this.textBox_time_ss.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Numeric_KeyPress_Shared);
            }
            if (this.textBox_time_ms != null)
            {
                this.textBox_time_ms.TextChanged += new System.EventHandler(this.TimeTextBox_TextChanged);
                this.textBox_time_ms.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Numeric_KeyPress_Shared);
            }

            if (this.button_set_start != null)
                this.button_set_start.Click += new System.EventHandler(this.button_set_start_Click);
            if (this.button_set_end != null)
                this.button_set_end.Click += new System.EventHandler(this.button_set_end_Click);
            if (this.button_save_cut != null)
                this.button_save_cut.Click += new System.EventHandler(this.button_save_cut_Click);

        }

        private async void TrimCutForm_Load(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentVideoPath) || _videoTotalDuration.TotalSeconds <= 0)
            {
                MessageBox.Show("Invalid video or zero duration.", "Video Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            if (trackBar_time_line != null)
            {
                trackBar_time_line.Minimum = 0;
                trackBar_time_line.Maximum = (int)Math.Max(0, _videoTotalDuration.TotalSeconds);
                trackBar_time_line.Value = 0;
            }

            UpdateStartTimeDisplay(_trimStartTime);
            UpdateEndTimeDisplay(_trimEndTime);
            UpdateTimeTextBoxes(_trimStartTime);

            if (pictureBox_video_preview != null)
            {
                pictureBox_video_preview.SizeMode = PictureBoxSizeMode.Zoom;
            }


            if (File.Exists(_ffmpegExecutablePath))
            {
                await ShowFrameAtTimeAsync(TimeSpan.Zero);
            }
            else
            {
                MessageBox.Show($"FFmpeg not found at: {_ffmpegExecutablePath}. Cannot display preview frame.", "FFmpeg Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTimeTextBoxes(TimeSpan currentTime)
        {
            if (textBox_time_hh == null || textBox_time_mm == null || textBox_time_ss == null || textBox_time_ms == null) return;

            _isUpdatingTimeFromTrackBar = true;
            textBox_time_hh.Text = currentTime.Hours.ToString("D2");
            textBox_time_mm.Text = currentTime.Minutes.ToString("D2");
            textBox_time_ss.Text = currentTime.Seconds.ToString("D2");
            textBox_time_ms.Text = currentTime.Milliseconds.ToString("D3");
            _isUpdatingTimeFromTrackBar = false;
        }

        private void UpdateStartTimeDisplay(TimeSpan startTime)
        {
            if (textBox_start_time == null) return;
            textBox_start_time.Text = startTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }

        private void UpdateEndTimeDisplay(TimeSpan endTime)
        {
            if (textBox_end_time == null) return;
            textBox_end_time.Text = endTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }

        private async Task ShowFrameAtTimeAsync(TimeSpan timeToShow)
        {
            _frameCaptureCts?.Cancel();
            _frameCaptureCts?.Dispose();
            _frameCaptureCts = new CancellationTokenSource();
            CancellationToken token = _frameCaptureCts.Token;

            string tempFramePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

            try
            {
                string timeFormatted = timeToShow.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
                string arguments = $"-y -ss {timeFormatted} -i \"{_currentVideoPath}\" -vframes 1 -an -f image2 \"{tempFramePath}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegExecutablePath,
                    Arguments = arguments,
                    WorkingDirectory = _ffmpegDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    StringBuilder errorOutput = new StringBuilder();
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorOutput.AppendLine(e.Data); };

                    process.Start();
                    process.BeginErrorReadLine();

                    await Task.Run(() => {
                        if (!process.WaitForExit(7000))
                        {
                            try { if (!process.HasExited) process.Kill(); } catch { /* ignore */ }
                            Debug.WriteLine("FFmpeg timed out for TrimCut preview.");
                        }
                    }, token);

                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Frame capture cancelled for TrimCut preview.");
                        return;
                    }

                    if (process.ExitCode == 0 && File.Exists(tempFramePath))
                    {
                        _currentFrameBitmap?.Dispose();
                        _currentFrameBitmap = null;

                        using (FileStream fs = new FileStream(tempFramePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await fs.CopyToAsync(ms, token);
                                ms.Position = 0;
                                _currentFrameBitmap = new Bitmap(ms);
                            }
                        }
                        if (pictureBox_video_preview != null) pictureBox_video_preview.Image = _currentFrameBitmap;
                    }
                    else
                    {
                        if (process.ExitCode != 0) Debug.WriteLine($"FFmpeg error (TrimCut) ExitCode: {process.ExitCode}. Errors: {errorOutput.ToString()}");
                        if (!File.Exists(tempFramePath) && process.ExitCode == 0) Debug.WriteLine("FFmpeg (TrimCut) completed but frame file not found.");
                    }
                }
            }
            catch (OperationCanceledException) { Debug.WriteLine("Frame capture operation cancelled."); }
            catch (Exception ex) { Debug.WriteLine($"Error in ShowFrameAtTimeAsync: {ex.Message}"); }
            finally
            {
                if (File.Exists(tempFramePath)) { try { File.Delete(tempFramePath); } catch { /* ignore */ } }
            }
        }

        private async void PreviewUpdateTimer_Tick(object? sender, EventArgs e)
        {
            _previewUpdateTimer.Stop();
            await ShowFrameAtTimeAsync(_lastTimeFromTrackBarOrTextBox);
        }

        private void trackBar_time_line_Scroll(object? sender, EventArgs e)
        {
            if (_isUpdatingTimeFromTextBox || trackBar_time_line == null) return;

            _isUpdatingTimeFromTrackBar = true;
            TimeSpan currentTime = TimeSpan.FromSeconds(trackBar_time_line.Value);
            UpdateTimeTextBoxes(currentTime);

            _lastTimeFromTrackBarOrTextBox = currentTime;
            _previewUpdateTimer.Stop();
            _previewUpdateTimer.Start();
            _isUpdatingTimeFromTrackBar = false;
        }

        private TimeSpan GetTimeFromTextBoxes()
        {
            int hours = int.TryParse(textBox_time_hh?.Text, out int h) ? h : 0;
            int minutes = int.TryParse(textBox_time_mm?.Text, out int m) ? m : 0;
            int seconds = int.TryParse(textBox_time_ss?.Text, out int s) ? s : 0;
            int milliseconds = int.TryParse(textBox_time_ms?.Text, out int msVal) ? msVal : 0;

            minutes = Math.Min(59, Math.Max(0, minutes));
            seconds = Math.Min(59, Math.Max(0, seconds));
            milliseconds = Math.Min(999, Math.Max(0, milliseconds));
            hours = Math.Max(0, hours);


            TimeSpan parsedTime = new TimeSpan(0, hours, minutes, seconds, milliseconds);

            if (parsedTime > _videoTotalDuration) parsedTime = _videoTotalDuration;
            if (parsedTime < TimeSpan.Zero) parsedTime = TimeSpan.Zero;
            return parsedTime;
        }

        private void TimeTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingTimeFromTrackBar || trackBar_time_line == null) return;

            _isUpdatingTimeFromTextBox = true;
            TimeSpan currentTime = GetTimeFromTextBoxes();

            int trackBarValue = (int)Math.Round(currentTime.TotalSeconds);

            if (trackBar_time_line.Value != trackBarValue)
            {
                trackBar_time_line.Value = Math.Clamp(trackBarValue, trackBar_time_line.Minimum, trackBar_time_line.Maximum);
            }

            _lastTimeFromTrackBarOrTextBox = currentTime;
            _previewUpdateTimer.Stop();
            _previewUpdateTimer.Start();
            _isUpdatingTimeFromTextBox = false;
        }

        private void TextBox_Numeric_KeyPress_Shared(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_set_start_Click(object? sender, EventArgs e)
        {
            if (trackBar_time_line == null) return;
            TimeSpan currentTime = TimeSpan.FromSeconds(trackBar_time_line.Value);

            if (currentTime > _trimEndTime && _videoTotalDuration.TotalSeconds > 0)
            {
                MessageBox.Show("The start time cannot be later than the current end time.", "Time Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _trimStartTime = currentTime;
            UpdateStartTimeDisplay(_trimStartTime);
        }

        private void button_set_end_Click(object? sender, EventArgs e)
        {
            if (trackBar_time_line == null) return;
            TimeSpan currentTime = TimeSpan.FromSeconds(trackBar_time_line.Value);

            if (currentTime < _trimStartTime && _videoTotalDuration.TotalSeconds > 0)
            {
                MessageBox.Show("The end time cannot be earlier than the current start time.", "Time Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _trimEndTime = (currentTime > _videoTotalDuration) ? _videoTotalDuration : currentTime;
            UpdateEndTimeDisplay(_trimEndTime);
        }

        private async void button_save_cut_Click(object? sender, EventArgs e)
        {
            if (button_save_cut == null) return;

            if (_trimStartTime >= _trimEndTime)
            {
                MessageBox.Show("The start time must be less than the end time to create a cut.", "Time Range Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TimeSpan cutDuration = _trimEndTime - _trimStartTime;
            if (cutDuration.TotalMilliseconds <= 0)
            {
                MessageBox.Show("The cut duration must be greater than 0.", "Cut Duration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string originalFileName = Path.GetFileNameWithoutExtension(_currentVideoPath);
            string originalExtension = Path.GetExtension(_currentVideoPath);
            string cutFileName = $"{originalFileName}_cut_{DateTime.Now:yyyyMMddHHmmssfff}{originalExtension}";

            string myVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string appSpecificCutFolder = Path.Combine(myVideosPath, "VideoLogoBatch Cuts");

            try
            {
                if (!Directory.Exists(appSpecificCutFolder))
                {
                    Directory.CreateDirectory(appSpecificCutFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create storage directory at: {appSpecificCutFolder}\nError: {ex.Message}", "Directory Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string outputCutFilePath = Path.Combine(appSpecificCutFolder, cutFileName);

            string startTimeFormatted = _trimStartTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
            string durationFormatted = cutDuration.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);

            string arguments = $"-y -ss {startTimeFormatted} -i \"{_currentVideoPath}\" -t {durationFormatted} -c copy -avoid_negative_ts make_zero \"{outputCutFilePath}\"";

            this.Text = "Processing video cut...";
            button_save_cut.Enabled = false;
            bool success = false;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegExecutablePath,
                    Arguments = arguments,
                    WorkingDirectory = _ffmpegDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    StringBuilder errorOutput = new StringBuilder();
                    process.ErrorDataReceived += (s, ev) => { if (ev.Data != null) errorOutput.AppendLine(ev.Data); };

                    process.Start();
                    process.BeginErrorReadLine();

                    await Task.Run(() => process.WaitForExit(300000));

                    if (!process.HasExited)
                    {
                        process.Kill();
                        throw new TimeoutException("The video cutting process took too long and was stopped!");
                    }

                    success = process.ExitCode == 0 && File.Exists(outputCutFilePath);
                    if (!success)
                    {
                        Debug.WriteLine($"Video cutting error. ExitCode: {process.ExitCode}. Errors: {errorOutput.ToString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while cutting video: {ex.Message}", "Video Cut Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                success = false;
            }

            if (success)
            {
                this.OutputCutFilePath = outputCutFilePath;
                this.DialogResult = DialogResult.OK;
                MessageBox.Show($"Cut file saved at: {outputCutFilePath}", "Cut Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Could not cut the video. Please check the parameters again.", "Video Cut Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = $"Trim/Cut - {Path.GetFileName(_currentVideoPath)}";
                button_save_cut.Enabled = true;
            }
        }

        private void TrimCutForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _previewUpdateTimer?.Stop();
            _previewUpdateTimer?.Dispose();

            _frameCaptureCts?.Cancel();
            _frameCaptureCts?.Dispose();

            _currentFrameBitmap?.Dispose();
            if (pictureBox_video_preview != null)
            {
                if (pictureBox_video_preview.Image != null && pictureBox_video_preview.Image != _currentFrameBitmap)
                {
                    pictureBox_video_preview.Image.Dispose();
                }
                pictureBox_video_preview.Image = null;
            }
        }

        private void button_cancel_trim_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}