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
using System.Threading.Channels;

namespace VideoPlayerClient.VideoStreamer
{
    public class VideoStreamerHttp(HttpClient httpClient) : IVideoStreamer
    {
        private readonly byte[] headerEndBytes = [(byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n'];
        private const string getVideoStreamEndpointUrl = "/mobile";
        private const string getCamerasEndpointUrl = "/configex";
        private readonly HttpClient _httpClient = httpClient;

        public async Task<BufferedStream> GetVideoStreamByIdAsync(string cameraId)
        {
            string queryParams = $"?login=root&channelid={cameraId}";

            var response = await _httpClient.GetStreamAsync($"{getVideoStreamEndpointUrl}{queryParams}").ConfigureAwait(false);

            BufferedStream streamReader = new(response);
            return streamReader;
        }

        public async Task<Dictionary<string, string>> GetCamerasAsync()
        {
            string queryParams = $"?login=root";
            var xml = await RecieveXmlConfigAsync($"{getCamerasEndpointUrl}{queryParams}");

            Dictionary<string, string> camerasInfo = ReadChansInfoFromXml(xml);
            return camerasInfo;
        }

        private async Task<XmlDocument> RecieveXmlConfigAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);

            var xmlString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(xmlString);
            return xmlResponse;
        }

        private Dictionary<string, string> ReadChansInfoFromXml(XmlDocument xml)
        {
            var channels = xml.GetElementsByTagName("ChannelInfo");
            Dictionary<string, string> result = [];
            if (channels == null)
            {
                return result;
            }

            for (int i = 0; i < channels.Count; i++)
            {
                var attrs = channels[i].Attributes ?? throw new Exception("channel info doesn't have attributes");

                var chanId = attrs.GetNamedItem("Id").Value ?? throw new Exception("channel info doesn't have Id attr");
                var chanName = attrs.GetNamedItem("Name").Value ?? throw new Exception("channel info doesn't have Name attr");
                result[chanId] = chanName;
            }
            return result;
        }

        public async Task<byte[]> GetVideoFrameAsync(BufferedStream stream)
        {
            string headerStr = await ReadHeaderFromStreamAsync(stream);
            int contentLength = GetContentLengthFromHeader(headerStr);

            byte[] imgData = await ReadImageRawFromStream(stream, contentLength);
            return imgData;
        }

        private async Task<string> ReadHeaderFromStreamAsync(BufferedStream stream)
        {
            // ищем конец хедера и возвращаем его
            Queue<byte> endHeaderBuffer = new(headerEndBytes.Length);
            byte[] currentByte = new byte[1];
            List<byte> header = [];

            while (!endHeaderBuffer.ToArray().SequenceEqual(headerEndBytes))
            {
                await stream.ReadAsync(currentByte.AsMemory(0, 1)).ConfigureAwait(false);
                if (endHeaderBuffer.Count >= headerEndBytes.Length)
                {
                    endHeaderBuffer.Dequeue();
                }
                endHeaderBuffer.Enqueue(currentByte[0]);
                header.Add(currentByte[0]);
            }

            string headerStr = new(header.Select(b=>(char)b).ToArray());
            return headerStr;
        }

        private int GetContentLengthFromHeader(string header)
        {
            var headerSplitted = header.Split("Content-Length:");
            if (headerSplitted.Length < 2)
            {
                throw new Exception("requested image doesn't have content type");
            }
            if (!int.TryParse(headerSplitted[1].Trim(), out int contentLength))
            {
                throw new Exception("requested image has invalid content type");
            }

            return contentLength;
        }

        private async Task<byte[]> ReadImageRawFromStream(BufferedStream stream, int contentLength)
        {
            byte[] imgData = new byte[contentLength];
            // занимает много времени и памяти, можно ли ускорить?
            await stream.ReadExactlyAsync(imgData.AsMemory(0, contentLength)).ConfigureAwait(false);
            return imgData;
        }
    }
}
