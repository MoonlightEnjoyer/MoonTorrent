using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TorrentCore
{
    public static class TorrentFileParser
    {
        public static byte[] CalculateInfoHash(string fileName)
        {
            using FileStream torrentFileStream = new FileStream(fileName, FileMode.Open);
            using BinaryReader binaryReader = new BinaryReader(torrentFileStream);
            var binaryContent = binaryReader.ReadBytes((int)torrentFileStream.Length);

            int startPos = -1;

            for (int i = 0; i < torrentFileStream.Length; i++)
            {
                if (binaryContent[i] == 'i' && binaryContent[i + 1] == 'n' && binaryContent[i + 2] == 'f' && binaryContent[i + 3] == 'o')
                {
                    startPos = i + 4;
                }
            }

            int endPos = 0;

            for (int i = 3; i < torrentFileStream.Length; i++)
            {
                if (binaryContent[torrentFileStream.Length - i] == 'e' && (binaryContent[torrentFileStream.Length - i + 1] >= '0' && binaryContent[torrentFileStream.Length - i + 1] <= '9'))
                {
                    endPos = (int)(torrentFileStream.Length - i + 1);
                    break;
                }
            }

            byte[] infoValue = binaryContent[startPos..endPos];
            string res = "";

            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(infoValue);

            return hash;
        }

        public static long GetTorrentSize(string fileName)
        {
            using StreamReader torrentStreamReader = new StreamReader(fileName);

            string fileContent = torrentStreamReader.ReadToEnd();

            fileContent = fileContent[(fileContent.IndexOf("files") + 5)..fileContent.IndexOf(":piece lengthi")];

            long torrentSize = 0;

            string rawLengthPattern = @"(lengthi\d+)";

            string lengthValuePattern = @"\d+";

            foreach (var match in Regex.Matches(fileContent, rawLengthPattern))
            {
                torrentSize += int.Parse(Regex.Match(match.ToString(), lengthValuePattern).ToString());
            }

            return torrentSize;
        }
    }
}
