using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoPlayerClient.Services.Interfaces;

namespace VideoPlayerClient.Services.MjpegReader
{
    internal class MjpegReader : IMjpegReader
    {
        public Task<BitmapImage> GetImageFromRawInputAsync(byte[] rawInput)
        {
            var img = new BitmapImage();

            using (var mem = new MemoryStream(rawInput))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = mem;
                img.EndInit();
                img.Freeze();
            }

            return Task.Run(() => { return img; });
        }
    }
}
