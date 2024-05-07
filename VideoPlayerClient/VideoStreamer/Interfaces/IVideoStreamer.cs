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
        public Task<BufferedStream> GetVideoStreamByIdAsync(string cameraId);

        public Task<byte[]> GetVideoFrameAsync(BufferedStream stream);
        public Task<Dictionary<string, string>> GetCamerasAsync();
    }
}
