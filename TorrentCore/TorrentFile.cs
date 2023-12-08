using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BencodeNET.Objects;

namespace TorrentCore
{
    public class TorrentFile
    {
        public string FileName { get; set; }

        private string UrlEncode()
        {
            string result = "";
            foreach (byte b in this.InfoHash)
            {
                if (char.IsAsciiLetterOrDigit((char)(b)) || b == '-' || b == '_' || b == '.' || b == '~')
                {
                    result += (char)b;
                }
                else
                {
                    result += "%" + Convert.ToHexString(new byte[] { b });
                }
            }

            return result;
        }

        public TorrentFile(string fileName, ushort port)
        {
            this.Announce = "http://bt.t-ru.org/ann";
            this.InfoHash = TorrentFileParser.CalculateInfoHash(fileName);
            this.InfoHashString = Convert.ToHexString(this.InfoHash);
            this.InfoHashUrlEncoded = this.UrlEncode();
            this.PeerId = "-PC0001-706887310628";
            this.Uploaded = 0;
            this.Downloaded = 0;
            this.Size = TorrentFileParser.GetTorrentSize(fileName);
            this.Left = this.Size - this.Downloaded;
            this.Port = port;
            this.Compact = 1;
            this.FragmentSize = 262144;
        }

        public string Announce {  get; set; }

        public byte[] InfoHash { get; set; }

        public string InfoHashString { get; set; }

        public string InfoHashUrlEncoded { get; set; }

        public string PeerId { get; set; }

        public ulong Uploaded { get; set; }

        public ulong Downloaded { get; set; }

        public ulong Left {  get; set; }

        public ushort Port { get; set; }

        public byte Compact { get; set; }

        public ulong Size {  get; set; }
        
        public uint FragmentSize {  get; set; }

        public List<FileInfo> Files { get; set; }
    }
}
