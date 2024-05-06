using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlayerClient.VideoStreamer.Interfaces;

namespace VideoPlayerClient.VideoStreamer
{
    public class VideoStreamerHttp : IVideoStreamer
    {
        public Task<byte[]> GetVideoFrame(StreamReader stream)
        {
            throw new NotImplementedException();
        }

        public Task<StreamReader> GetVideoStreamByIdAsync(string cameraId)
        {
            throw new NotImplementedException();
        }
    }
}
