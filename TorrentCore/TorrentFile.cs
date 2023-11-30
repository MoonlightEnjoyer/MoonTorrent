﻿using System;
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
            foreach (char ch in this.InfoHash)
            {
                if (char.IsAsciiLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '~')
                {
                    result += ch;
                }
                else
                {
                    result += "%" + (byte)ch;
                }
            }

            return result;
        }

        public TorrentFile(string fileName, ushort port)
        {
            this.Announce = "http://bt.t-ru.org/ann";
            this.InfoHash = TorrentFileParser.CalculateInfoHash(fileName);
            this.InfoHashUrlEncoded = this.UrlEncode();
            this.PeerId = "QWERTYUIOPASDFGHJKLZ";
            this.Uploaded = 0;
            this.Downloaded = 0;
            this.Left = TorrentFileParser.GetTorrentSize(fileName);
            this.Port = port;
            this.Compact = 1;
        }

        public string Announce {  get; set; }

        public string InfoHash { get; set; }

        public string InfoHashUrlEncoded { get; set; }

        public string PeerId { get; set; }

        public long Uploaded { get; set; }

        public long Downloaded { get; set; }

        public long Left {  get; set; }

        public ushort Port { get; set; }

        public byte Compact { get; set; }
    }
}
