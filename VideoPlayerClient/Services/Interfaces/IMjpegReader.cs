using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoPlayerClient.Services.Interfaces
{
    public interface IMjpegReader
    {
        public Task<BitmapImage> GetImageFromRawInputAsync(byte[] rawInput);
    }
}
