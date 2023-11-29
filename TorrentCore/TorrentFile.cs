using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentCore
{
    public class TorrentFile
    {
        public string FileName { get; set; }

        public TorrentFile(string fileName, ushort port)
        {
            this.InfoHash = TorrentFileParser.CalculateInfoHash(fileName);
            this.PeerId = "QWERTYUIOPASDFGHJKLZ";
            this.Uploaded = 0;
            this.Downloaded = 0;
            this.Left = 0;
            this.Port = port;
            this.Compact = 1;
        }

        public string InfoHash { get; set; }

        public string PeerId { get; set; }

        public long Uploaded { get; set; }

        public long Downloaded { get; set; }

        public long Left {  get; set; }

        public ushort Port { get; set; }

        public byte Compact { get; set; }
    }
}
