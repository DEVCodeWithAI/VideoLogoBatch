using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace VideoLogoBatch
{
    public partial class Main : Form
    {
        private Task? _renderTask = null;
        private CancellationTokenSource? _renderCancellationTokenSource = null;
        private Process? _currentFFmpegProcess = null;

        private int _totalVideosToRender = 0;
        private int _videosSuccessfullyRenderedCount = 0;
        private int _currentVideoProcessingIndex = 0;

        private readonly string _ffmpegExecutablePath = string.Empty;
        private readonly string _ffprobeExecutablePath = string.Empty;
        private readonly string _ffmpegDirectory = string.Empty;


        public Main()
        {
            InitializeComponent();
            InitializeControlLinking();
            this.FormClosed += Main_FormClosed;

            string ffmpegRelativePath = @"ffmpeg\bin\ffmpeg.exe";
            string ffprobeRelativePath = @"ffmpeg\bin\ffprobe.exe";

            _ffmpegExecutablePath = Path.Combine(Application.StartupPath, ffmpegRelativePath);
            _ffprobeExecutablePath = Path.Combine(Application.StartupPath, ffprobeRelativePath);
            _ffmpegDirectory = Path.GetDirectoryName(_ffmpegExecutablePath) ?? Application.StartupPath;

            bool ffmpegExists = File.Exists(_ffmpegExecutablePath);
            bool ffprobeExists = File.Exists(_ffprobeExecutablePath);

            if (!ffmpegExists || !ffprobeExists)
            {
                string missingFiles = "";
                if (!ffmpegExists) missingFiles += $"\n - FFmpeg: '{_ffmpegExecutablePath}'";
                if (!ffprobeExists) missingFiles += $"\n - FFprobe: '{_ffprobeExecutablePath}'";

                MessageBox.Show($"The following required executable files are missing: {missingFiles}\nThe rendering feature will be disabled.", "Missing File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                renderControl1.DisableRenderButton();
            }
        }

        private void InitializeControlLinking()
        {
            videoSourceControl1.VideoFileSelectedForPreview += async (sender, videoPath) =>
            {
                if (!string.IsNullOrEmpty(videoPath))
                {
                    await previewControl1.LoadVideoFrameAsync(videoPath);
                    ApplyLogoToPreview();
                }
                else
                {
                    previewControl1.ClearPreviewResources();
                }
            };

            logoOptionsControl1.LogoSettingsChanged += (sender, eventArgs) =>
            {
                ApplyLogoToPreview();
            };

            renderControl1.RequestRenderStart += RenderControl1_RequestRenderStart;
            renderControl1.RequestRenderStop += RenderControl1_RequestRenderStop;
            renderControl1.RequestOpenLog += RenderControl1_RequestOpenLog;
        }

        private void ApplyLogoToPreview()
        {
            if (previewControl1.IsVideoPreviewLoaded())
            {
                var logoSettings = logoOptionsControl1.GetCurrentLogoSettings();
                previewControl1.UpdateLogoOnPreview(logoSettings);
            }
            else
            {
                previewControl1.UpdateLogoOnPreview(null);
            }
        }


        private void RenderControl1_RequestRenderStart(object? sender, EventArgs e)
        {
            if (!File.Exists(_ffmpegExecutablePath) || !File.Exists(_ffprobeExecutablePath))
            {
                MessageBox.Show("FFmpeg/FFprobe could not be found. Rendering cannot be started.", "Missing File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                renderControl1.SetRenderIdle();
                return;
            }

            List<string> videosToRender = videoSourceControl1.GetSelectedVideoFullPaths();
            if (!videosToRender.Any()) videosToRender = videoSourceControl1.GetAllVideoFullPaths();

            if (!videosToRender.Any())
            {
                MessageBox.Show("Please add at least one video to the list to proceed with rendering.", "No Video Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                renderControl1.SetRenderIdle();
                return;
            }

            var logoSettings = logoOptionsControl1.GetCurrentLogoSettings();
            string encoderOption = renderControl1.SelectedEncoder;
            string formatString = renderControl1.SelectedFormat;

            string actualOutputDirectory = renderControl1.ActualOutputDirectory;
            if (string.IsNullOrEmpty(actualOutputDirectory) || !Directory.Exists(actualOutputDirectory))
            {
                try
                {
                    Directory.CreateDirectory(actualOutputDirectory);
                    if (!Directory.Exists(actualOutputDirectory))
                    {
                        MessageBox.Show($"The output folder could not be accessed or created: {actualOutputDirectory}. Please verify the path and try again.", "Output Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        renderControl1.SetRenderIdle();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the output folder: {actualOutputDirectory}\nDetails: {ex.Message}", "Output Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    renderControl1.SetRenderIdle();
                    return;
                }
            }


            string fileExtension = formatString.Contains("(") && formatString.Contains(")")
                                   ? formatString.Substring(formatString.LastIndexOf('(') + 1).TrimEnd(')')
                                   : ".mp4";

            _renderCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _renderCancellationTokenSource.Token;

            renderControl1.LogMessage($"RENDER SESSION STARTED: {DateTime.Now}", true);
            renderControl1.LogMessage($"Output Directory: {actualOutputDirectory}");
            renderControl1.LogMessage($"Encoder: {encoderOption}");
            renderControl1.LogMessage($"Format: {formatString} (Extension: {fileExtension})");
            renderControl1.LogMessage($"Logo Path: {logoSettings.Path ?? "N/A"}");
            if (!string.IsNullOrEmpty(logoSettings.Path))
            {
                renderControl1.LogMessage($"Logo Position: {(logoSettings.IsRandomPosition ? "Floating Effect" : logoSettings.Position)}");
                renderControl1.LogMessage($"Logo Size: {logoSettings.SizePercent}%");
                renderControl1.LogMessage($"Logo Opacity: {logoSettings.OpacityPercent}%");
                renderControl1.LogMessage($"Logo Margin: {logoSettings.Margin}");
            }
            renderControl1.LogMessage($"Videos to process: {videosToRender.Count}");
            foreach (var video in videosToRender)
            {
                renderControl1.LogMessage($"  - {Path.GetFileName(video)}");
            }

            _totalVideosToRender = videosToRender.Count;
            _videosSuccessfullyRenderedCount = 0;
            _currentVideoProcessingIndex = 0;
            string initialProgressMessage = $"Processing... video 1/{_totalVideosToRender}";
            if (_totalVideosToRender == 0) initialProgressMessage = "Status: No videos to process.";
            else if (_totalVideosToRender == 1) initialProgressMessage = $"Processing... video 1/1";

            renderControl1.UpdateRenderProgressText(initialProgressMessage, true, false, false);

            _renderTask = Task.Run(async () =>
            {
                for (int i = 0; i < _totalVideosToRender; i++)
                {
                    _currentVideoProcessingIndex = i;
                    if (token.IsCancellationRequested)
                    {
                        renderControl1.LogMessage($"Render cancelled by user before processing video: {Path.GetFileName(videosToRender[i])}");
                        break;
                    }

                    this.Invoke((Action)(() => UpdateUIRenderProgress()));

                    string currentVideoPath = videosToRender[i];

                    string baseOutputFileNameWithoutExt = $"{Path.GetFileNameWithoutExtension(currentVideoPath)}_logo";
                    string outputFileNameWithExt = $"{baseOutputFileNameWithoutExt}{fileExtension}";
                    string finalOutputFilePath = Path.Combine(actualOutputDirectory, outputFileNameWithExt);

                    int count = 1;
                    while (File.Exists(finalOutputFilePath))
                    {
                        outputFileNameWithExt = $"{baseOutputFileNameWithoutExt} ({count}){fileExtension}";
                        finalOutputFilePath = Path.Combine(actualOutputDirectory, outputFileNameWithExt);
                        count++;
                    }

                    renderControl1.LogMessage($"Processing video: {Path.GetFileName(currentVideoPath)} ({i + 1}/{_totalVideosToRender}) -> {Path.GetFileName(finalOutputFilePath)}");

                    var videoDimensions = await GetVideoDimensionsAsync(currentVideoPath, token);
                    if (!videoDimensions.HasValue)
                    {
                        renderControl1.LogMessage($"Error: Failed to get video dimensions for: {Path.GetFileName(currentVideoPath)}. This video will be skipped.");
                        continue;
                    }

                    string ffmpegCommandArgs = BuildFFmpegCommand(currentVideoPath, finalOutputFilePath, logoSettings, encoderOption, videoDimensions.Value.Width);

                    renderControl1.LogMessage($"DEBUG FFmpeg Command String: ffmpeg {ffmpegCommandArgs}");

                    bool success = await ExecuteFFmpegCommandAsync(ffmpegCommandArgs, token);

                    if (token.IsCancellationRequested)
                    {
                        renderControl1.LogMessage($"Render cancelled by user during/after processing video: {Path.GetFileName(currentVideoPath)}");
                        break;
                    }

                    if (success)
                    {
                        _videosSuccessfullyRenderedCount++;
                        renderControl1.LogMessage($"Successfully processed: {Path.GetFileName(finalOutputFilePath)}");
                    }
                    else
                    {
                        renderControl1.LogMessage($"Failed to process: {Path.GetFileName(currentVideoPath)}. Check FFmpeg output above for errors.");
                    }
                }
            }, token).ContinueWith(task =>
            {
                bool wasCancelledByUser = token.IsCancellationRequested || (task.IsCanceled || (task.IsFaulted && task.Exception?.GetBaseException() is OperationCanceledException));

                if (wasCancelledByUser)
                {
                    renderControl1.LogMessage("RENDER SESSION STOPPED BY USER.");
                    ActualUpdateUIRenderProgress(true);
                }
                else if (task.IsFaulted)
                {
                    renderControl1.LogMessage($"RENDER SESSION FAILED WITH ERROR: {task.Exception?.GetBaseException().Message}");
                    ActualUpdateUIRenderProgress(false, task.Exception);
                }
                else
                {
                    _currentVideoProcessingIndex = _totalVideosToRender;
                    renderControl1.LogMessage($"RENDER SESSION COMPLETED. {_videosSuccessfullyRenderedCount}/{_totalVideosToRender} videos processed successfully.");
                    System.Media.SystemSounds.Exclamation.Play();
                    MessageBox.Show("Render Completed!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualUpdateUIRenderProgress();
                }
                _renderCancellationTokenSource?.Dispose();
                _renderCancellationTokenSource = null;
                _currentFFmpegProcess = null;
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void UpdateUIRenderProgress(bool userStopped = false, Exception? overallException = null)
        {
            ActualUpdateUIRenderProgress(userStopped, overallException);
        }

        private void ActualUpdateUIRenderProgress(bool userStopped = false, Exception? overallException = null)
        {
            string progressText;

            if (userStopped)
            {
                progressText = $"Status: Stopped at video {_currentVideoProcessingIndex + 1}/{_totalVideosToRender} by user.";
            }
            else if (overallException != null && !(_renderCancellationTokenSource != null && _renderCancellationTokenSource.IsCancellationRequested))
            {
                progressText = $"Status: Error at video {_currentVideoProcessingIndex + 1}/{_totalVideosToRender}.";
            }
            else if (_renderCancellationTokenSource != null && !_renderCancellationTokenSource.IsCancellationRequested &&
                     _videosSuccessfullyRenderedCount < _totalVideosToRender && _totalVideosToRender > 0)
            {
                progressText = $"Processing... video {_currentVideoProcessingIndex + 1}/{_totalVideosToRender}";
            }
            else if (_videosSuccessfullyRenderedCount == _totalVideosToRender && _totalVideosToRender > 0)
            {
                progressText = $"Status: Completed {_videosSuccessfullyRenderedCount}/{_totalVideosToRender} videos successfully.";
            }
            else if (_currentVideoProcessingIndex >= _totalVideosToRender && _totalVideosToRender > 0)
            {
                progressText = $"Status: Finished. {_videosSuccessfullyRenderedCount}/{_totalVideosToRender} videos processed successfully.";
            }
            else if (_totalVideosToRender == 0)
            {
                progressText = "Status: No videos to process.";
            }
            else
            {
                progressText = "Status: Idle";
            }

            bool isRunning = _renderCancellationTokenSource != null &&
                             !_renderCancellationTokenSource.IsCancellationRequested &&
                             overallException == null && !userStopped &&
                             _videosSuccessfullyRenderedCount < _totalVideosToRender && _totalVideosToRender > 0;

            bool completedSuccessfully = !isRunning && !userStopped && overallException == null &&
                                         _videosSuccessfullyRenderedCount == _totalVideosToRender && _totalVideosToRender > 0;

            renderControl1.UpdateRenderProgressText(progressText, isRunning, completedSuccessfully, userStopped);

            if (overallException != null && !userStopped && !(overallException.GetBaseException() is OperationCanceledException))
            {
                MessageBox.Show($"An unexpected critical error occurred during rendering:\n{overallException.GetBaseException().Message}", "Critical Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task<(int Width, int Height)?> GetVideoDimensionsAsync(string videoPath, CancellationToken token)
        {
            string arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{videoPath}\"";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffprobeExecutablePath,
                    Arguments = arguments,
                    WorkingDirectory = _ffmpegDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
                {
                    var tcs = new TaskCompletionSource<(int, string, string)>();
                    token.Register(() => {
                        tcs.TrySetCanceled();
                        SafelyKillProcess(process);
                    });

                    process.Exited += (s, e) => tcs.TrySetResult((process.ExitCode, string.Empty, string.Empty));

                    StringBuilder outputBuilder = new StringBuilder();
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };

                    process.Start();
                    process.BeginOutputReadLine();

                    try
                    {
                        await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, token));
                        if (tcs.Task.IsCanceled) return null;
                    }
                    catch (OperationCanceledException)
                    {
                        renderControl1.LogMessage($"Video dimension retrieval cancelled for: {Path.GetFileName(videoPath)}");
                        return null;
                    }


                    if (process.ExitCode == 0)
                    {
                        string output = outputBuilder.ToString().Trim();
                        string[] dimensions = output.Split('x');
                        if (dimensions.Length == 2 && int.TryParse(dimensions[0], out int width) && int.TryParse(dimensions[1], out int height))
                        {
                            return (width, height);
                        }
                        else
                        {
                            renderControl1.LogMessage($"Error parsing video dimensions from ffprobe output: '{output}' for video '{Path.GetFileName(videoPath)}'");
                        }
                    }
                    else
                    {
                        string errorOutput = await process.StandardError.ReadToEndAsync(token);
                        renderControl1.LogMessage($"ffprobe encountered an error while retrieving video dimensions (ExitCode: {process.ExitCode}): {errorOutput} for video '{Path.GetFileName(videoPath)}'");
                    }
                }
            }
            catch (Exception ex)
            {
                renderControl1.LogMessage($"An error occurred while running ffprobe to retrieve video dimensions: {ex.Message} for video '{Path.GetFileName(videoPath)}'");
            }
            return null;
        }

        private string BuildFFmpegCommand(string inputVideoPath, string outputVideoPath,
                                  LogoOptionsControl.LogoSettingsData logoSettings,
                                  string encoderOption, int videoWidth)
        {
            StringBuilder commandBuilder = new StringBuilder();

            commandBuilder.Append("-hide_banner -y ");

            if (encoderOption.Contains("NVIDIA (NVENC)"))
            {
                commandBuilder.Append("-hwaccel cuda ");
            }
            commandBuilder.Append($"-i \"{inputVideoPath}\" ");

            string videoFilterComplexContent = string.Empty;
            string finalVideoMapLabel = "[0:v]";

            bool hasLogo = !string.IsNullOrEmpty(logoSettings.Path) && File.Exists(logoSettings.Path);

            if (hasLogo)
            {
                commandBuilder.Append($"-i \"{logoSettings.Path}\" ");

                string logoInputStream = "[1:v]";

                double sizeMultiplier = logoSettings.SizePercent / 100.0;
                if (sizeMultiplier <= 0) sizeMultiplier = 0.01;
                int logoPixelWidth = (int)(videoWidth * sizeMultiplier);
                if (logoPixelWidth < 2) logoPixelWidth = 2;
                logoPixelWidth = (logoPixelWidth / 2) * 2;

                double opacityValue = logoSettings.OpacityPercent / 100.0;
                string opacityStr = opacityValue.ToString("0.##", CultureInfo.InvariantCulture);

                if (logoSettings.IsRandomPosition)
                {
                    string processedLogoLabel = "[logo_float_processed]";
                    string filterChainForLogo = $"{logoInputStream}format=rgba,colorchannelmixer=aa={opacityStr},scale=w={logoPixelWidth}:h=-2{processedLogoLabel}";

                    int margin = logoSettings.Margin;
                    string xOverlayExpr = $"'{margin}+mod(t/5*100,main_w-overlay_w-({margin}*2))'";
                    string yOverlayExpr = $"'(main_h-overlay_h)/2+sin(t/5*2)*((main_h-overlay_h)/3)'";

                    finalVideoMapLabel = "[dynamic_logo_out]";
                    string filterChainForOverlay = $"[0:v]{processedLogoLabel}overlay=x={xOverlayExpr}:y={yOverlayExpr},format=yuv420p{finalVideoMapLabel}";

                    videoFilterComplexContent = $"{filterChainForLogo};{filterChainForOverlay}";
                }
                else
                {
                    List<string> staticFilters = new List<string>();
                    string currentProcessedLogoLabel = logoInputStream;

                    staticFilters.Add($"{currentProcessedLogoLabel}scale=w={logoPixelWidth}:h=-2[scaled_static_logo]");
                    currentProcessedLogoLabel = "[scaled_static_logo]";

                    if (logoSettings.OpacityPercent < 100)
                    {
                        staticFilters.Add($"{currentProcessedLogoLabel}format=rgba,colorchannelmixer=aa={opacityStr}[logo_op_static]");
                        currentProcessedLogoLabel = "[logo_op_static]";
                    }

                    string positionName = logoSettings.Position;
                    string xPos = "10", yPos = "10";
                    int margin = logoSettings.Margin;
                    switch (positionName)
                    {
                        case "Top Left": xPos = $"{margin}"; yPos = $"{margin}"; break;
                        case "Top Center": xPos = $"(main_w-overlay_w)/2"; yPos = $"{margin}"; break;
                        case "Top Right": xPos = $"main_w-overlay_w-{margin}"; yPos = $"{margin}"; break;
                        case "Bottom Left": xPos = $"{margin}"; yPos = $"main_h-overlay_h-{margin}"; break;
                        case "Bottom Center": xPos = $"(main_w-overlay_w)/2"; yPos = $"main_h-overlay_h-{margin}"; break;
                        case "Bottom Right": xPos = $"main_w-overlay_w-{margin}"; yPos = $"main_h-overlay_h-{margin}"; break;
                        case "Center": xPos = "(main_w-overlay_w)/2"; yPos = "(main_h-overlay_h)/2"; break;
                    }

                    finalVideoMapLabel = "[static_logo_out]";
                    staticFilters.Add($"[0:v]{currentProcessedLogoLabel}overlay=x={xPos}:y={yPos},format=yuv420p{finalVideoMapLabel}");

                    videoFilterComplexContent = string.Join(",", staticFilters);
                }
            }

            if (!string.IsNullOrEmpty(videoFilterComplexContent))
            {
                commandBuilder.Append($"-filter_complex \"{videoFilterComplexContent}\" ");
                commandBuilder.Append($"-map \"{finalVideoMapLabel}\" ");
            }
            else
            {
                if (encoderOption.Contains("NVIDIA (NVENC)") && !hasLogo)
                {
                    commandBuilder.Append("-vf \"format=yuv420p\" ");
                }
                commandBuilder.Append($"-map {finalVideoMapLabel} ");
            }

            string videoCodec = "libx264";
            string preset = "medium";
            string qualityParam = "-crf 23";

            if (encoderOption.Contains("NVIDIA (NVENC)"))
            {
                videoCodec = "h264_nvenc";
                qualityParam = "-cq 23";
                preset = "p6";
            }
            else if (encoderOption.Contains("AMD (AMF)"))
            {
                videoCodec = "h264_amf";
                qualityParam = "-quality quality";
                preset = "balanced";
            }

            commandBuilder.Append($"-c:v {videoCodec} ");
            if (!string.IsNullOrEmpty(preset))
            {
                commandBuilder.Append($"-preset {preset} ");
            }
            commandBuilder.Append($"{qualityParam} ");

            commandBuilder.Append("-map 0:a? -c:a copy ");
            commandBuilder.Append($"\"{outputVideoPath}\"");

            return commandBuilder.ToString();
        }

        private async Task<bool> ExecuteFFmpegCommandAsync(string arguments, CancellationToken token)
        {
            _currentFFmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegExecutablePath,
                    Arguments = arguments,
                    WorkingDirectory = _ffmpegDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            using (_currentFFmpegProcess)
            {
                var processCompletionSource = new TaskCompletionSource<bool>();
                token.Register(() => {
                    processCompletionSource.TrySetCanceled();
                    SafelyKillProcess(_currentFFmpegProcess);
                });

                _currentFFmpegProcess.Exited += (s, e) => {
                    if (!token.IsCancellationRequested) processCompletionSource.TrySetResult(_currentFFmpegProcess.ExitCode == 0);
                };

                _currentFFmpegProcess.ErrorDataReceived += (s, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        this.Invoke((Action)(() => renderControl1.LogMessage($"FFMPEG: {e.Data}")));
                    }
                };
                _currentFFmpegProcess.OutputDataReceived += (s, e) => {
                    // Standard output usually doesn't need detailed logging, unless debugging
                    // if (!string.IsNullOrEmpty(e.Data)) this.Invoke((Action)(() => renderControl1.LogMessage($"FFMPEG_OUT: {e.Data}")));
                };

                renderControl1.LogMessage($"Executing FFmpeg: {_ffmpegExecutablePath} {arguments}");

                try
                {
                    _currentFFmpegProcess.Start();
                    _currentFFmpegProcess.BeginErrorReadLine();
                    _currentFFmpegProcess.BeginOutputReadLine();
                    return await processCompletionSource.Task;
                }
                catch (TaskCanceledException)
                {
                    renderControl1.LogMessage("FFmpeg process cancellation was requested (TaskCanceledException).");
                    return false;
                }
                catch (Exception ex)
                {
                    renderControl1.LogMessage($"Exception during FFmpeg execution: {ex.Message}");
                    SafelyKillProcess(_currentFFmpegProcess);
                    return false;
                }
            }
        }
        private void SafelyKillProcess(Process? process)
        {
            if (process != null)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(true);
                        renderControl1.LogMessage("FFmpeg process kill signal sent successfully.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    renderControl1.LogMessage($"Info trying to kill FFmpeg process (it might have already exited): {ex.Message}");
                }
                catch (Exception ex)
                {
                    renderControl1.LogMessage($"Error trying to kill FFmpeg process: {ex.Message}");
                }
            }
        }


        private void RenderControl1_RequestRenderStop(object? sender, EventArgs e)
        {
            if (_renderCancellationTokenSource != null && !_renderCancellationTokenSource.IsCancellationRequested)
            {
                renderControl1.LogMessage("Stop request received. Attempting to cancel render process...");
                _renderCancellationTokenSource.Cancel();
            }
            else
            {
                renderControl1.LogMessage("Stop request received, but no active cancellable render process found or already cancelling.");
                ActualUpdateUIRenderProgress(true);
            }
        }

        private void RenderControl1_RequestOpenLog(object? sender, EventArgs e)
        {
            string logFilePath = renderControl1.CurrentLogFilePath;
            if (!string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(logFilePath) { UseShellExecute = true };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open the log file: {logFilePath}\nDetails: {ex.Message}", "Log File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No log file found or no render sessions have been run yet.", "Log Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void OpenFileToolStripMenuItem_Click(object? sender, EventArgs e) { /* Handled in VideoSourceControl */ }
        private void OpenFolderToolStripMenuItem_Click(object? sender, EventArgs e) { /* Handled in VideoSourceControl */ }
        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e) { Application.Exit(); }
        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Video Logo Batch\nVersion 1.0.0\n\n" +
                "Developed by Trí Trần (DevCodeWithAI)\n" +
                "Licensed under the Apache License 2.0\n\n" +
                "GitHub: https://github.com/DEVCodeWithAI/VideoLogoBatch\n\n" +
                "Note: This app requires FFmpeg. Please download it separately from https://ffmpeg.org",
                "About Video Logo Batch",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Main_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (_renderCancellationTokenSource != null && !_renderCancellationTokenSource.IsCancellationRequested)
            {
                _renderCancellationTokenSource.Cancel();
                SafelyKillProcess(_currentFFmpegProcess);
            }
            previewControl1.ClearPreviewResources();
        }

        public async Task<TimeSpan?> GetVideoDurationForPathAsync(string videoPath) // Public for TrimCutForm
        {
            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath) || !File.Exists(_ffprobeExecutablePath))
            {
                return null;
            }

            string arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{videoPath}\"";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffprobeExecutablePath,
                    Arguments = arguments,
                    WorkingDirectory = _ffmpegDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
                {
                    var tcs = new TaskCompletionSource<string?>();
                    var ctsTimeout = new CancellationTokenSource(5000);
                    ctsTimeout.Token.Register(() => tcs.TrySetCanceled());

                    process.Exited += (s, ev) => tcs.TrySetResult(null);

                    StringBuilder outputBuilder = new StringBuilder();
                    process.OutputDataReceived += (s, ev) => { if (ev.Data != null) outputBuilder.AppendLine(ev.Data); };

                    process.Start();
                    process.BeginOutputReadLine();

                    await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, ctsTimeout.Token));
                    if (tcs.Task.IsCanceled && !process.HasExited) { SafelyKillProcess(process); return null; }

                    if (process.ExitCode == 0)
                    {
                        string durationStr = outputBuilder.ToString().Trim();
                        if (double.TryParse(durationStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double seconds))
                        {
                            return TimeSpan.FromSeconds(seconds);
                        }
                        LogMessageForTrimCut($"[ERROR] Unable to extract duration using ffprobe. Raw value: '{durationStr}' | File: '{Path.GetFileName(videoPath)}'");
                    }
                    else
                    {
                        string errorOutput = await process.StandardError.ReadToEndAsync(ctsTimeout.Token);
                        LogMessageForTrimCut($"ffprobe failed to retrieve duration (ExitCode: {process.ExitCode}): {errorOutput} | Video: '{Path.GetFileName(videoPath)}'");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessageForTrimCut($"Exception in GetVideoDurationForPathAsync: {ex.Message} | Video: '{Path.GetFileName(videoPath)}'");
            }
            return null;
        }
        private void LogMessageForTrimCut(string message)
        {
            Debug.WriteLine($"[TrimCutContext] {message}");
            // Alternatively, use renderControl1.LogMessage to log in the main render output panel
        }
    }
}
