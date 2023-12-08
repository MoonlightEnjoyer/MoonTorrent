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


        public static ulong GetTorrentSize(string fileName)
        {
            using StreamReader torrentStreamReader = new StreamReader(fileName);

            string fileContent = torrentStreamReader.ReadToEnd();

            fileContent = fileContent[(fileContent.IndexOf("files") + 5)..fileContent.IndexOf(":piece lengthi")];

            ulong torrentSize = 0;

            string rawLengthPattern = @"(lengthi\d+)";

            string lengthValuePattern = @"\d+";

            foreach (var match in Regex.Matches(fileContent, rawLengthPattern))
            {
                torrentSize += uint.Parse(Regex.Match(match.ToString(), lengthValuePattern).ToString());
            }

            return torrentSize;
        }

        public static List<FileInfo> GetTorrentFilesInfo(string fileName)
        {
            List<FileInfo> files = new List<FileInfo>();

            using StreamReader torrentStreamReader = new StreamReader(fileName);

            string fileContent = torrentStreamReader.ReadToEnd();

            fileContent = fileContent[(fileContent.IndexOf("files") + 5)..fileContent.IndexOf(":piece lengthi")];

            ulong torrentSize = 0;

            string rawLengthPattern = @"(lengthi\d+e\d+:pathl\d+:[\w\d\.\-\s']+ee\w\d+:)";

            string lengthValuePattern = @"\d+";

            string fileNamePattern = @"pathl\d+.+";

            int startPos = 0;

            while (true)
            {
                startPos = fileContent.IndexOf("lengthi", startPos);
                if (startPos == -1)
                {
                    break;
                }

                var endPos = fileContent.IndexOf("lengthi", startPos + 1);

                if (endPos == -1)
                {
                    endPos = fileContent.IndexOf("eee4:", startPos + 1);
                }


                if (endPos == -1)
                {
                    break;
                }

                string rawInfo = fileContent[startPos..endPos];

                int lengthStart = 7;
                int lengthEnd = rawInfo.IndexOf("e4");

                int size = int.Parse(rawInfo[lengthStart..lengthEnd]);

                int nameLength = int.Parse(Regex.Match(rawInfo[(lengthEnd + 3)..], @"\d+").ToString());

                int nameStart = rawInfo.IndexOf(":", lengthEnd + 3) + 1;

                string name = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(rawInfo)[nameStart..(nameStart + nameLength)]);

                startPos = endPos;


                files.Add(new FileInfo { Name = name, Size = size });
                Console.WriteLine($"{name} : {size}");
            }

            return files;
        }
    }
}
