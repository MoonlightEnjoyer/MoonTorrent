using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentCore
{
    public class HandshakePacket
    {

        public HandshakePacket(byte[] infoHash, string peerId)
        {
            this.Pstrlen = 19;
            this.Pstr = "BitTorrent protocol";
            this.Reserved = [0, 0, 0, 0, 0, 0, 0, 0];
            this.InfoHash = infoHash;
            this.PeerId = Encoding.UTF8.GetBytes(peerId);
        }

        public byte Pstrlen {  get; set; }

        public string Pstr { get; set; }

        public byte[] Reserved { get; set; }

        public byte[] InfoHash { get; set; }

        public byte[] PeerId {  get; set; }

        public byte[] BuildPacket()
        {
            byte[] packetBytes = new byte[1 + 19 + 8 + 20 + 20];

            packetBytes[0] = this.Pstrlen;
            Encoding.UTF8.GetBytes(this.Pstr).CopyTo(packetBytes, 1);
            this.Reserved.CopyTo(packetBytes, 20);
            this.InfoHash.CopyTo(packetBytes, 28);
            this.PeerId.CopyTo(packetBytes, 48);

            return packetBytes;
        }
    }
}
