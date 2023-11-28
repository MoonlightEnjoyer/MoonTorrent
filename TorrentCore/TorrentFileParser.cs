using System.Security.Cryptography;
using System.Text;

namespace TorrentCore
{
    public class TorrentFileParser
    {
        public string CalculateInfoHash(string fileName)
        {
            using StreamReader torrentFile = new StreamReader(fileName);
            string fileContent = torrentFile.ReadToEnd();
            int startPos = fileContent.IndexOf("info") + 4;
            int endPos = 0;
 
            for(int i=3;i<fileContent.Length;i++)
            {
                if(fileContent[fileContent.Length-i]=='e' && (fileContent[fileContent.Length-i+1]>='0' && fileContent[fileContent.Length-i+1]<='9'))
                {
                    endPos=i;
                    break;
                }
            }
            

            string infoValue = fileContent[startPos..endPos];

            using var sha1 = SHA1.Create();
            return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(infoValue)));
        }
    }
}
