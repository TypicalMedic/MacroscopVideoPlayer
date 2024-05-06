using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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

        #region CamerasIds
        private Dictionary<string, string> _CamerasIds = [];

        public Dictionary<string, string> CamerasIds
        {
            get => _CamerasIds;
            set => Set(ref _CamerasIds, value);
        }
        #endregion

        #region SelectedCam
        private string _SelectedCam = string.Empty;

        public string SelectedCam
        {
            get => _SelectedCam;
            set => Set(ref _SelectedCam, value);
        }
        #endregion

        #region Img
        private Image _Img = new Image();

        public Image Img
        {
            get => _Img;
            set => Set(ref _Img, value);
        }
        #endregion

        #region GetSelectedVideoCommand

        public ICommand GetVidCommand { get; set; }

        private async void OnGetSelectedVideoCommandExecuted(object? p)
        {
            if (SelectedCam.Equals(string.Empty))
            {
                throw new Exception("no cam selected!");
            }
            var imgRaw = await _videoStreamerService.GetVideoFrameFromStream(SelectedCam);

            var bitmapImg = await _mjpegReader.GetImageFromRawInputAsync(imgRaw);
           
            Img.Source = bitmapImg;
        }
        private bool CanGetSelectedVideoCommandExecute(object? p) => true; //todo?

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
