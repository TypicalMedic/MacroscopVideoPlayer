using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VideoPlayerClient.Commands;
using VideoPlayerClient.Services.Interfaces;
using VideoPlayerClient.VideoStreamer.Interfaces;
using VideoPlayerClient.ViewModels.Base;

namespace VideoPlayerClient.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IVideoStreamerService _videoStreamerService;
        private readonly IMjpegReader _mjpegReader;
        private CancellationTokenSource CTS = new CancellationTokenSource();
        private readonly object _lockCts = new object();
        private const int camCount = 3;

        #region CamerasIds
        private Dictionary<string, string> _CamerasIds = [];

        public Dictionary<string, string> CamerasIds
        {
            get => _CamerasIds;
            set => Set(ref _CamerasIds, value);
        }
        #endregion

        #region SelectedCams
        private string[] _SelectedCams = new string[camCount];

        public string[] SelectedCams
        {
            get => _SelectedCams;
            set => Set(ref _SelectedCams, value);
        }

        #endregion

        #region Imgs
        private ObservableCollection<ImageSource?> _Imgs = [new BitmapImage(), new BitmapImage(), new BitmapImage()];

        public ObservableCollection<ImageSource?> Imgs
        {
            get => _Imgs;
            set => Set(ref _Imgs, value);
        }
        #endregion

        #region GetSelectedVideoCommand

        public ICommand GetVidCommand { get; set; }

        private void OnGetSelectedVideoCommandExecuted(object? p)
        {
            if (_SelectedCams.Any(s => s.Equals(string.Empty)))
            {
                MessageBox.Show("select all cams!");
                return;
            }

            ResetCancellationToken();

            LoadCameras();
        }

        private void LoadCameras()
        {
            for (int i = 0; i < camCount; i++)
            {
                int index = i;
                IProgress<ImageSource> progress = new Progress<ImageSource>(src => Imgs[index] = src);

                Task.Run(async () =>
                {
                    var imgsRaw = _videoStreamerService.GetVideoFrameFromStreamRawAsync(SelectedCams[index]);

                    var imgs = ProcessImgsAsync(imgsRaw, index);
                    await foreach (var img in imgs)
                    {
                        progress.Report(img);
                    }
                });
            }
        }

        private void ResetCancellationToken()
        {
            // костыль?
            lock (_lockCts)
            {
                CTS.Cancel();
                CTS.Dispose();
                CTS = new CancellationTokenSource();
            }
        }

        private async IAsyncEnumerable<BitmapImage> ProcessImgsAsync(IAsyncEnumerable<byte[]> imgs, int index)
        {
            await foreach (var imgRaw in imgs.WithCancellation(CTS.Token))
            {
                var bitmapImg = await _mjpegReader.GetImageFromRawInputAsync(imgRaw);
                Debug.WriteLine(index + " " + bitmapImg);
                yield return bitmapImg;
            }
        }

        private bool CanGetSelectedVideoCommandExecute(object? p) => true;

        #endregion

        public MainWindowViewModel(IVideoStreamerService videoStreamerService, IMjpegReader mjpegReader)
        {
            GetVidCommand = new LambdaCommand(OnGetSelectedVideoCommandExecuted, CanGetSelectedVideoCommandExecute);

            _videoStreamerService = videoStreamerService;
            _mjpegReader = mjpegReader;

            Task.Run(() => ConfigureCamerasIdsAsync());
        }

        private async Task ConfigureCamerasIdsAsync()
        {
            var ids = await GetCamerasIdsAsync();

            SetCamerasIdsSelect(ids);
        }

        private void SetCamerasIdsSelect(Dictionary<string, string> ids)
        {
            CamerasIds = ids;
        }

        private async Task<Dictionary<string, string>> GetCamerasIdsAsync()
        {
            var ids = await _videoStreamerService.GetCameras();
            return ids;
        }
    }
}
