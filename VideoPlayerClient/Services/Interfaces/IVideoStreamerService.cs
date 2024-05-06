using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerClient.Services.Interfaces
{
    public interface IVideoStreamerService
    {
        public Task<IAsyncEnumerable<byte[]>> GetVideoFromStream(string cameraId);

        public Task<List<string>> GetCameras();
    }
}
