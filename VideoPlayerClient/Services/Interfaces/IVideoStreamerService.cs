using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerClient.Services.Interfaces
{
    public interface IVideoStreamerService
    {
        public Task<char[]> GetVideoFromStream(string cameraId);

        public Task<Dictionary<string, string>> GetCameras();
    }
}
