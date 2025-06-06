using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Diagnostics;

namespace VideoLogoBatch
{
    public partial class RenderControl : UserControl
    {
        public event EventHandler? RequestRenderStart;
        public event EventHandler? RequestRenderStop;
        public event EventHandler? RequestOpenLog;

        private string _userSelectedOutputDirectory = string.Empty;
        private const string AppSpecificOutputSubfolderName = "VideoLogoBatch_Output";
        private string _currentLogFilePath = string.Empty;

        public RenderControl()
        {
            InitializeComponent();
            InitializeRenderSettings();

            button_output_folder.Click += Button_output_folder_Click;
            button_start_render.Click += Button_start_render_Click;
            button_stop_render.Click += Button_stop_render_Click;
            button_openlog.Click += Button_openlog_Click;
        }

        private void InitializeRenderSettings()
        {
            DetectAndSetEncoder();

            if (comboBox_format.Items.Contains("MP4 (.mp4)"))
            {
                comboBox_format.SelectedItem = "MP4 (.mp4)";
            }
            else if (comboBox_format.Items.Count > 0)
            {
                comboBox_format.SelectedIndex = 0;
            }

            SetDefaultOutputFolderDisplay();

            label_processing_render.Text = "Status: Idle";
            button_stop_render.Enabled = false;
        }

        private void DetectAndSetEncoder()
        {
            string preferredEncoder = "CPU (libx264)";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    string adapterName = mo["Name"]?.ToString()?.ToUpperInvariant() ?? "";
                    if (adapterName.Contains("NVIDIA"))
                    {
                        preferredEncoder = "NVIDIA (NVENC)";
                        break;
                    }
                    if (adapterName.Contains("AMD") || adapterName.Contains("RADEON"))
                    {
                        preferredEncoder = "AMD (AMF)";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to detect GPU: {ex.Message}");
                MessageBox.Show("Unable to detect GPU. Defaulting to CPU encoder.", "Encoder Detection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (comboBox__encoder.Items.Contains(preferredEncoder))
            {
                comboBox__encoder.SelectedItem = preferredEncoder;
            }
            else if (comboBox__encoder.Items.Count > 0)
            {
                comboBox__encoder.SelectedIndex = 0;
            }
        }

        private void SetDefaultOutputFolderDisplay()
        {
            string myVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string defaultOutputPath = Path.Combine(myVideosPath, AppSpecificOutputSubfolderName);
            UpdateOutputFolderDisplay(defaultOutputPath);
        }

        private void UpdateOutputFolderDisplay(string folderPath)
        {
            textBox_output_folder.Text = folderPath;
        }

        private void Button_output_folder_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the root folder to save processed videos. A subfolder named 'VideoLogoBatch_Output' will be created if it does not already exist.";
                folderDialog.ShowNewFolderButton = true;

                string currentBaseFolder = textBox_output_folder.Text;
                if (!string.IsNullOrEmpty(currentBaseFolder) && currentBaseFolder.EndsWith(Path.DirectorySeparatorChar + AppSpecificOutputSubfolderName))
                {
                    currentBaseFolder = Path.GetDirectoryName(currentBaseFolder) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                }
                else if (string.IsNullOrEmpty(currentBaseFolder) || !Directory.Exists(currentBaseFolder))
                {
                    currentBaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                }
                folderDialog.SelectedPath = currentBaseFolder;


                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    _userSelectedOutputDirectory = folderDialog.SelectedPath;
                    UpdateOutputFolderDisplay(Path.Combine(_userSelectedOutputDirectory, AppSpecificOutputSubfolderName));
                }
            }
        }

        private void Button_start_render_Click(object? sender, EventArgs e)
        {
            string finalOutputBaseDirectory;
            if (!string.IsNullOrEmpty(_userSelectedOutputDirectory) && Directory.Exists(_userSelectedOutputDirectory))
            {
                finalOutputBaseDirectory = _userSelectedOutputDirectory;
            }
            else
            {
                finalOutputBaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }

            string actualOutputDirectoryWithSubfolder = Path.Combine(finalOutputBaseDirectory, AppSpecificOutputSubfolderName);

            try
            {
                Directory.CreateDirectory(actualOutputDirectoryWithSubfolder);
                UpdateOutputFolderDisplay(actualOutputDirectoryWithSubfolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to access or create the output folder:\n{actualOutputDirectoryWithSubfolder}\n\nDetails: {ex.Message}",
                    "Output Folder Issue",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            _currentLogFilePath = Path.Combine(logDirectory, $"render_{DateTime.Now:yyyyMMdd_HHmmssfff}.log");

            button_start_render.Enabled = false;
            button_stop_render.Enabled = true;
            label_processing_render.Text = "Status: Starting...";
            RequestRenderStart?.Invoke(this, EventArgs.Empty);
        }

        private void Button_stop_render_Click(object? sender, EventArgs e)
        {
            RequestRenderStop?.Invoke(this, EventArgs.Empty);
        }

        private void Button_openlog_Click(object? sender, EventArgs e)
        {
            RequestOpenLog?.Invoke(this, EventArgs.Empty);
        }

        public string SelectedEncoder => comboBox__encoder.SelectedItem?.ToString() ?? "CPU (libx264)";
        public string SelectedFormat => comboBox_format.SelectedItem?.ToString() ?? "MP4 (.mp4)";

        public string ActualOutputDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(textBox_output_folder.Text) && Directory.Exists(textBox_output_folder.Text))
                {
                    return textBox_output_folder.Text;
                }
                string baseDir = !string.IsNullOrEmpty(_userSelectedOutputDirectory) && Directory.Exists(_userSelectedOutputDirectory)
                                 ? _userSelectedOutputDirectory
                                 : Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                return Path.Combine(baseDir, AppSpecificOutputSubfolderName);
            }
        }
        public string CurrentLogFilePath => _currentLogFilePath;

        public void UpdateRenderProgressText(string progressMessage, bool isRunning, bool completedSuccessfully, bool stoppedByUser)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => {
                    label_processing_render.Text = progressMessage;
                    button_start_render.Enabled = !isRunning;
                    button_stop_render.Enabled = isRunning;
                }));
            }
            else
            {
                label_processing_render.Text = progressMessage;
                button_start_render.Enabled = !isRunning;
                button_stop_render.Enabled = isRunning;
            }
        }

        public void SetRenderIdle()
        {
            label_processing_render.Text = "Status: Idle";
            button_start_render.Enabled = true;
            button_stop_render.Enabled = false;
        }

        public void LogMessage(string message, bool appendToUiLog = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentLogFilePath))
                {
                    File.AppendAllText(_currentLogFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while writing log: {ex.Message}");
            }
        }
        public void DisableRenderButton()
        {
            this.button_start_render.Enabled = false;
        }
    }
}
