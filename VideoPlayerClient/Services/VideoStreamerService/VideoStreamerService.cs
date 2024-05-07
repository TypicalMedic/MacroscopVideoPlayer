using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public async IAsyncEnumerable<byte[]> GetVideoFrameFromStreamRawAsync(string cameraId, [EnumeratorCancellation] CancellationToken token = default)
        {
            BufferedStream? stream = null;
            while (!token.IsCancellationRequested)
            {
                stream = await _videoStreamer.GetVideoStreamByIdAsync(cameraId);
                var img = await _videoStreamer.GetVideoFrameAsync(stream);
                yield return img;
            }
            stream?.Close();
        }
    }
}
