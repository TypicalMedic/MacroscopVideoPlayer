using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerClient.Services.Interfaces
{
    public interface IVideoStreamerService
    {
        public IAsyncEnumerable<byte[]> GetVideoFrameFromStreamRawAsync(string cameraId, CancellationToken token = default);

        public Task<Dictionary<string, string>> GetCameras();
    }
}
