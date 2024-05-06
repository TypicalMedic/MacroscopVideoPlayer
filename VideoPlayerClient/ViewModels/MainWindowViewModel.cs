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

        #region CamerasIds
        private List<string> _CamerasIds { get; set; } = [];

        public List<string> CamerasIds
        {
            get => _CamerasIds;
            set => _CamerasIds = value;
        }
        #endregion


        #region Img
        private Image _Img { get; set; } = new Image();

        public Image Img
        {
            get => _Img;
            set => _Img = value;
        }
        #endregion

        #region GetSelectedVideoCommand

        public ICommand GetVidCommand { get; set; }

        private async void OnGetSelectedVideoCommandExecuted(object? p)
        {
            Console.WriteLine("test");
            // async 

            // Get video stream from API

            // Get video frame from steam

            // Update img with frame

            // how to end stream????????????????????
        }
        private bool CanGetSelectedVideoCommandExecute(object? p) => true; //todo?

        #endregion

        public MainWindowViewModel(IVideoStreamerService videoStreamerService)
        {
            GetVidCommand = new LambdaCommand(OnGetSelectedVideoCommandExecuted, CanGetSelectedVideoCommandExecute);

            _videoStreamerService = videoStreamerService;

            Task.Run(() => ConfigureCamerasIdsAsync());
        }

        private async Task ConfigureCamerasIdsAsync()
        {
            var ids = await GetCamerasIdsAsync();

            SetCamerasIdsSelect(ids);
        }

        private void SetCamerasIdsSelect(List<string> ids)
        {
            _CamerasIds = ids;
        }

        private async Task<List<string>> GetCamerasIdsAsync()
        {
            var ids = await _videoStreamerService.GetCameras();
            return ids;
        }
    }
}
