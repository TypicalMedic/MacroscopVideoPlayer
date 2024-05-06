using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerClient.VideoStreamer.Interfaces
{
    public interface IVideoStreamer
    {
        public Task<StreamReader> GetVideoStreamByIdAsync(string cameraId);

        public Task<byte[]> GetVideoFrame(StreamReader stream);
    }
}
