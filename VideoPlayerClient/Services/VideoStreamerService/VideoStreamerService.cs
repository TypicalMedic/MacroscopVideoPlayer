using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlayerClient.Services.Interfaces;
using VideoPlayerClient.VideoStreamer.Interfaces;

namespace VideoPlayerClient.Services.VideoStreamerService
{
    public class VideoStreamerService(IVideoStreamer videoStreamer) : IVideoStreamerService
    {
        private readonly IVideoStreamer _videoStreamer = videoStreamer;
        public async Task<Dictionary<string, string>> GetCameras()
        {
            var cameras = await _videoStreamer.GetCamerasAsync();
            return cameras;
        }

        public async Task<byte[]> GetVideoFrameFromStreamRawAsync(string cameraId)
        {
            var stream = await _videoStreamer.GetVideoStreamByIdAsync(cameraId);
            var img = await _videoStreamer.GetVideoFrameAsync(stream);

            return img;
        }

    }
}
