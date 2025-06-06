using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoLogoBatch
{
    public partial class VideoSourceControl : UserControl
    {
        public event EventHandler<string?>? VideoFileSelectedForPreview;

        private string? _currentPreviewedVideoPath = null;

        private class VideoFileItem
        {
            public string? DisplayName { get; set; }
            public string? FullPath { get; set; }
            public TimeSpan? UserTrimStartTime { get; set; }
            public TimeSpan? UserTrimEndTime { get; set; }
            public string? ProcessedSegmentPath { get; set; }

            public override string ToString()
            {
                if (UserTrimStartTime.HasValue && UserTrimEndTime.HasValue)
                {
                    return $"{DisplayName} [Trimmed: {UserTrimStartTime:hh\\:mm\\:ss} - {UserTrimEndTime:hh\\:mm\\:ss}]";
                }
                return DisplayName ?? string.Empty;
            }
        }

        private readonly List<string> _supportedVideoExtensions = new List<string>
        {
            ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm", ".mpeg", ".mpg", ".ts",
            ".vob", ".mts", ".m2ts", ".3gp"
        };

        public VideoSourceControl()
        {
            InitializeComponent();
            button_trim_cut.Click += Button_trim_cut_Click;

            listBox_video_source.SelectedIndexChanged += ListBox_video_source_SelectedIndexChanged;

            if (this.openFolderToolStripMenuItem != null)
            {
                this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenFolderToolStripMenuItem_Click);
            }
            this.listBox_video_source.MouseDown += new MouseEventHandler(this.listBox_video_source_MouseDown);
        }

        private void Button_import_file_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string filterExtensions = string.Join(";", _supportedVideoExtensions.Select(ext => $"*{ext}"));
                openFileDialog.Filter = $"Video Files ({filterExtensions})|{filterExtensions}|All files (*.*)|*.*";
                openFileDialog.Title = "Import Video File(s)";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        AddVideoToList(filePath);
                    }
                }
            }
        }

        private void listBox_video_source_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBox_video_source.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBox_video_source.SelectedIndex = index;
                }
            }
        }

        private void OpenFolderToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (listBox_video_source.SelectedItem is VideoFileItem selectedVideoItem &&
                !string.IsNullOrEmpty(selectedVideoItem.FullPath))
            {
                try
                {
                    string? directoryPath = Path.GetDirectoryName(selectedVideoItem.FullPath);
                    if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                    {
                        Process.Start("explorer.exe", directoryPath);
                    }
                    else
                    {
                        MessageBox.Show($"Could not find the directory for the file: {selectedVideoItem.FullPath}", "Open Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening directory: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Button_import_folder_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select Folder Containing Videos";
                folderBrowserDialog.ShowNewFolderButton = false;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolderPath = folderBrowserDialog.SelectedPath;
                    try
                    {
                        var videoFiles = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                                                  .Where(f => _supportedVideoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

                        foreach (string filePath in videoFiles)
                        {
                            AddVideoToList(filePath);
                        }

                        if (!videoFiles.Any())
                        {
                            MessageBox.Show("No supported video files found in the selected folder.", "Import Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error accessing folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddVideoToList(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) || !_supportedVideoExtensions.Contains(Path.GetExtension(fullPath).ToLowerInvariant()))
            {
                return;
            }

            foreach (VideoFileItem existingItem in listBox_video_source.Items)
            {
                if (existingItem.FullPath != null && existingItem.FullPath.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            string? displayName = Path.GetFileName(fullPath);
            if (displayName == null)
            {
                return;
            }

            var videoItem = new VideoFileItem
            {
                DisplayName = displayName,
                FullPath = fullPath
            };
            listBox_video_source.Items.Add(videoItem);
        }

        private void Button_clean_video_list_Click(object? sender, EventArgs e)
        {
            listBox_video_source.Items.Clear();
            _currentPreviewedVideoPath = null;
            VideoFileSelectedForPreview?.Invoke(this, null);
        }

        private void Button_delete_select_video_list_Click(object? sender, EventArgs e)
        {
            if (listBox_video_source.SelectedItems.Count > 0)
            {
                bool previewedItemWasAmongDeleted = false;
                List<VideoFileItem> itemsToRemove = new List<VideoFileItem>();

                foreach (VideoFileItem selectedItem in listBox_video_source.SelectedItems)
                {
                    itemsToRemove.Add(selectedItem);
                    if (_currentPreviewedVideoPath != null && selectedItem.FullPath == _currentPreviewedVideoPath)
                    {
                        previewedItemWasAmongDeleted = true;
                    }
                }

                foreach (VideoFileItem itemToRemove in itemsToRemove)
                {
                    listBox_video_source.Items.Remove(itemToRemove);
                }

                if (previewedItemWasAmongDeleted)
                {
                    _currentPreviewedVideoPath = null;
                    VideoFileSelectedForPreview?.Invoke(this, null);
                }
            }
        }

        private void Button_select_all_video_list_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < listBox_video_source.Items.Count; i++)
            {
                listBox_video_source.SetSelected(i, true);
            }
        }

        private void ListBox_video_source_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBox_video_source.SelectedItem is VideoFileItem selectedVideo)
            {
                _currentPreviewedVideoPath = selectedVideo.FullPath;
                VideoFileSelectedForPreview?.Invoke(this, selectedVideo.FullPath);
            }
            else
            {
                _currentPreviewedVideoPath = null;
                VideoFileSelectedForPreview?.Invoke(this, null);
            }
        }

        private async void Button_trim_cut_Click(object? sender, EventArgs e)
        {
            if (listBox_video_source.SelectedItems.Count == 1)
            {
                int selectedIndex = listBox_video_source.SelectedIndex;
                VideoFileItem? selectedVideoItem = listBox_video_source.SelectedItem as VideoFileItem;

                if (selectedVideoItem != null && !string.IsNullOrEmpty(selectedVideoItem.FullPath) && File.Exists(selectedVideoItem.FullPath))
                {
                    Main? mainForm = this.FindForm() as Main;
                    if (mainForm == null)
                    {
                        MessageBox.Show("Cannot determine the main application form.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    TimeSpan? videoDuration = await mainForm.GetVideoDurationForPathAsync(selectedVideoItem.FullPath);

                    if (videoDuration.HasValue && videoDuration.Value.TotalSeconds > 0)
                    {
                        using (TrimCutForm trimCutWindow = new TrimCutForm(selectedVideoItem.FullPath, videoDuration.Value))
                        {
                            DialogResult result = trimCutWindow.ShowDialog(mainForm);
                            if (result == DialogResult.OK)
                            {
                                TimeSpan startTime = trimCutWindow.UserTrimStartTime;
                                TimeSpan endTime = trimCutWindow.UserTrimEndTime;
                                string? cutFilePath = trimCutWindow.OutputCutFilePath;

                                selectedVideoItem.UserTrimStartTime = startTime;
                                selectedVideoItem.UserTrimEndTime = endTime;

                                if (!string.IsNullOrEmpty(cutFilePath) && File.Exists(cutFilePath))
                                {
                                    string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedVideoItem.FullPath);

                                    selectedVideoItem.FullPath = cutFilePath;
                                    selectedVideoItem.DisplayName = $"{originalFileNameWithoutExtension}_cut";
                                }

                                listBox_video_source.Items[selectedIndex] = selectedVideoItem;


                                MessageBox.Show($"Trim/Cut information for the video has been updated.\n" +
                                                $"New Path: {selectedVideoItem.FullPath}\n" +
                                                $"Start: {startTime:hh\\:mm\\:ss\\.fff}\n" +
                                                $"End: {endTime:hh\\:mm\\:ss\\.fff}",
                                                "Trim/Cut Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                VideoFileSelectedForPreview?.Invoke(this, selectedVideoItem.FullPath);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not get the duration of the selected video, or the video is too short.", "Video Duration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid video file from the list.", "Video Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (listBox_video_source.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select only one video to perform Trim/Cut.", "Select One Video", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a video from the list to perform Trim/Cut.", "No Video Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public List<string> GetSelectedVideoFullPaths()
        {
            var selectedPaths = new List<string>();
            if (listBox_video_source.SelectedItems.Count > 0)
            {
                foreach (VideoFileItem selectedItem in listBox_video_source.SelectedItems)
                {
                    if (selectedItem.FullPath != null)
                    {
                        selectedPaths.Add(selectedItem.FullPath);
                    }
                }
            }
            return selectedPaths;
        }

        public List<string> GetAllVideoFullPaths()
        {
            var allPaths = new List<string>();
            if (listBox_video_source.Items.Count > 0)
            {
                foreach (VideoFileItem item in listBox_video_source.Items)
                {
                    if (item.FullPath != null)
                    {
                        allPaths.Add(item.FullPath);
                    }
                }
            }
            return allPaths;
        }
    }
}