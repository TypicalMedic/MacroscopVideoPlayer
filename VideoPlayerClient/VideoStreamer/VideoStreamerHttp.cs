using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using VideoPlayerClient.VideoStreamer.Interfaces;

namespace VideoPlayerClient.VideoStreamer
{
    public class VideoStreamerHttp(HttpClient httpClient) : IVideoStreamer
    {
        private char[] headerEndBytes = { '\r', '\n', '\r', '\n' };
        private const string getVideoStreamEndpointUrl = "/mobile";
        private const string getCamerasEndpointUrl = "/configex";
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Dictionary<string, string>> GetCamerasAsync()
        {
            string queryParams = $"?login=root";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{getCamerasEndpointUrl}{queryParams}");
            var response = await _httpClient.SendAsync(request);

            var xmlString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(xmlString);

            var channels = xmlResponse.GetElementsByTagName("ChannelInfo");

            Dictionary<string, string> result = [];

            for(int i = 0; i < channels.Count; i++)
            {
                var chanId = channels[i]?.Attributes?.GetNamedItem("Id").Value;
                var chanName = channels[i]?.Attributes?.GetNamedItem("Name").Value;
                result[chanId] = chanName; 
            }            

            return result;
        }

        public async Task<char[]> GetVideoFrameAsync(StreamReader stream)
        {
            // читаем 1 байт
            // дбавляем в хедер
            // стаивм в конец буффера
            // убираем 1 байт начала буффера
            // сверяем с концом хедера
            // если ок читаем картинку

            Queue<char> endHeaderBuffer = new(headerEndBytes.Length);
            char[] currentChar = new char[1];
            List<char> header = [];

            while (!endHeaderBuffer.ToArray().SequenceEqual(headerEndBytes))
            {
                await stream.ReadAsync(currentChar, 0, 1).ConfigureAwait(false);
                if (endHeaderBuffer.Count >= headerEndBytes.Length)
                {
                    endHeaderBuffer.Dequeue();
                }
                endHeaderBuffer.Enqueue(currentChar[0]);
                header.Add(currentChar[0]);
            }

            string headerStr = new(header.ToArray());
            var headerSplitted = headerStr.Split("Content-Length:");
            if (headerSplitted.Length < 2)
            {
                throw new Exception("requested image doesn't have content type");
            }
            if (!int.TryParse(headerSplitted[1].Trim(), out int contentLength))
            {
                throw new Exception("requested image has invalid content type");
            }

            char[] imgData = new char[contentLength];
            await stream.ReadAsync(imgData, 0, contentLength).ConfigureAwait(false);

            return imgData;
        }

        public async Task<StreamReader> GetVideoStreamByIdAsync(string cameraId)
        {
            string queryParams = $"?login=root&channelid={cameraId}";

            var response = await _httpClient.GetStreamAsync($"{getVideoStreamEndpointUrl}{queryParams}").ConfigureAwait(false);

            StreamReader streamReader = new(response);
            return streamReader;
        }
    }
}
