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

        public Task<char[]> GetVideoFrameAsync(StreamReader stream);
        public Task<Dictionary<string, string>> GetCamerasAsync();
    }
}
