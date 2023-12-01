using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;

namespace TorrentCore
{
    public class TrackerCommunication
    {
        public void GetPeers(TorrentFile torrentFile)
        {
            string url = @$"{torrentFile.Announce}?info_hash={torrentFile.InfoHashUrlEncoded}&peer_id={torrentFile.PeerId}&uploaded=0&downloaded=0&left={torrentFile.Left}&port={torrentFile.Port}&compact=1";
            var userAgent = new ProductInfoHeaderValue("MoonTorrent", "0.1");
            var clientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.UserAgent.Add(userAgent);
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            client.DefaultRequestHeaders.Connection.ParseAdd("Close");
            
            using HttpResponseMessage response = client.GetAsync(url).Result;
            byte[] peersData = response.Content.ReadAsByteArrayAsync().Result;
            byte[] pattern = Encoding.UTF8.GetBytes("peers");

            ushort peersDataStart = 0;

            for (ushort i = 0; i < peersData.Length; i++)
            {
                if (peersData.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    peersDataStart = i;
                    break;
                }
            }

            peersDataStart += 5;

            for (ushort i = peersDataStart; i < peersData.Length; i++)
            {
                if (peersData[i] == ':')
                {
                    peersDataStart = i;
                    peersDataStart += 1;
                    break;
                }
            }

            Console.WriteLine(peersDataStart);
            Console.WriteLine(Encoding.UTF8.GetString(peersData));

            for (int i = peersDataStart; i + 5 < peersData.Length; i += 6)
            {
                byte[] ip = new byte[4];

                for (int j = i; j < i + 4; j++)
                {
                    ip[j - i] = BinaryPrimitives.ReverseEndianness(peersData[j]);
                }

                

                ushort port = 0;
                port |= BinaryPrimitives.ReverseEndianness(peersData[i + 4]);
                port <<= 8;
                port |= BinaryPrimitives.ReverseEndianness(peersData[i + 5]);

                Console.WriteLine($"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]} : {port}");
            }
        }
    }
}
