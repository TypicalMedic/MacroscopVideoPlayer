using System;
using System.Collections.Generic;
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
        public Task<List<string>> GetCameras()
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncEnumerable<byte[]>> GetVideoFromStream(string cameraId)
        {
            throw new NotImplementedException();
        }
    }
}
