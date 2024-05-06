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
        public Task<BitmapImage> GetImageFromRawInputAsync(char[] rawInput)
        {
            var rawByteInput = rawInput.Select(c => (byte)c).ToArray();
            //rawByteInput[0] = (byte)0xFF;
            //rawByteInput[1] = (byte)0xD8;
            //rawByteInput[2] = (byte)0xFF;
            //rawByteInput[3] = (byte)0xE0;

            var img = new BitmapImage();

            using (var mem = new MemoryStream(rawByteInput))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = mem;
                img.EndInit();
                img.Freeze();
            }

            return Task.Run(() => { return img; });
            throw new NotImplementedException();
        }
    }
}
