using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;

namespace TorrentCore
{
    public class PeerCommunication
    {

        public void SendHandshake(IPEndPoint peer, HandshakePacket handshakePacket, TorrentFile torrentFile)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(peer);
                NetworkStream stream = client.GetStream();
                stream.Write(handshakePacket.BuildPacket());

                byte[] buffer = new byte[4096];
                var bytesRead = stream.Read(buffer, 0, 4096);

                Console.WriteLine($"{peer.Address} : {bytesRead} bytes");

                ushort blockSize = 16384;

                ulong bytesDownloaded = 0;

                if (bytesRead > 0)
                {
                    //1. send interested message + wait choke/unchoke +
                    bool unchoke = this.SendInterested(stream);

                    if (unchoke)
                    {
                        Console.WriteLine($"{peer.Address} : unchoked");

                        using FileStream fileStream = new FileStream("F:\\MoonTorrent\\result", FileMode.Create);
                        using BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                        uint fragmentNumber = 0;
                        //2. download file part dedicated for this peer session
                        while (bytesDownloaded < torrentFile.Size)
                        {
                            uint offset = 0;
                            while (offset < torrentFile.FragmentSize && bytesDownloaded + offset < torrentFile.Size)
                            {
                                if (blockSize <= torrentFile.FragmentSize - offset)
                                {
                                    offset += this.RequestPiece(ref stream, binaryWriter, fragmentNumber, offset, blockSize);
                                }
                                else
                                {
                                    offset += this.RequestPiece(ref stream, binaryWriter, fragmentNumber, offset, (ushort)(torrentFile.FragmentSize - offset));
                                }

                            }

                            fragmentNumber++;
                            bytesDownloaded += offset;

                            Console.WriteLine($"bytesDownloaded : {bytesDownloaded} #######################################################");
                        }

                        //while (true)
                        //{
                        //    uint fragmentCounter = 0;
                        //    uint blockNumber = 0;
                        //    while (fragmentCounter < torrentFile.FragmentSize)
                        //    {
                        //        if (blockSize <= torrentFile.FragmentSize - fragmentCounter)
                        //        {
                        //            fragmentCounter += this.RequestPiece(ref stream, binaryWriter, fragmentNumber, blockNumber, blockSize);
                        //        }
                        //        else
                        //        {
                        //            fragmentCounter += this.RequestPiece(ref stream, binaryWriter, fragmentNumber, blockNumber, (ushort)(torrentFile.FragmentSize - fragmentCounter));
                        //        }

                        //        blockNumber++;
                        //    }

                        //    fragmentNumber++;
                        //    bytesDownloaded += fragmentCounter;

                        //    Console.WriteLine($"bytesDownloaded : {bytesDownloaded} #######################################################");
                        //}
                    }
                    else
                    {
                        Console.WriteLine($"{peer.Address} : choked");
                    }
                }
                
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                //Console.WriteLine("Exceprion occured.");
            }
        }

        private bool SendInterested(NetworkStream stream)
        {
            byte[] message = [0, 0, 0, BinaryPrimitives.ReverseEndianness((byte)1), 2];

           

            byte[] buffer = new byte[4096];

            stream.Write(message);

            int counter = 0;

            do
            {
                int bytesRead = stream.Read(buffer);

                Console.WriteLine($"bytes read interested: {bytesRead}");

                counter++;

                if (counter > 10)
                {
                    return false;
                }
            }
            while (BinaryPrimitives.ReverseEndianness(buffer[3]) != 1);

            if (buffer[4] == 0)
            {
                return false;
            }

            return buffer[4] == 1;
        }

        private uint RequestPiece(ref NetworkStream stream, BinaryWriter binaryWriter, uint index, uint offset, ushort blockSize)
        {
            Console.WriteLine("Requesting piece");

            byte[] message = {0, 0, 0, BinaryPrimitives.ReverseEndianness((byte)13), 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var indexBytes = BitConverter.GetBytes((ushort)index).Reverse().ToArray();
            indexBytes.CopyTo(message, 5 + 4 - indexBytes.Length);

            var offsetBytes = BitConverter.GetBytes(offset).Reverse().ToArray();
            offsetBytes.CopyTo(message, 9 + 4 - offsetBytes.Length);

            var blockSizeBytes = BitConverter.GetBytes(blockSize).Reverse().ToArray();
            blockSizeBytes.CopyTo(message, 13 + 4 - blockSizeBytes.Length);


            Console.WriteLine($"fragment : {message[5]}_{message[6]}_{message[7]}_{message[8]}");
            Console.WriteLine($"offset : {message[9]}_{message[10]}_{message[11]}_{message[12]}");

            stream.Write(message);

            byte[] buffer = new byte[300000];

            uint totalBytes = 0;

            while (totalBytes < blockSize + 13)
            {
                int bytesRead = stream.Read(buffer, (int)totalBytes, blockSize);

                if (bytesRead != 0)
                {
                    totalBytes += (uint)bytesRead;

                    //Console.WriteLine($"bytesRead = {bytesRead}");
                    //Console.WriteLine($"totalBytes = {totalBytes}");
                    //Console.WriteLine($"message id = {buffer[4]}");
                }
            }

            Console.WriteLine($"totalBytes = {totalBytes}");

            binaryWriter.Write(buffer, 13, (int)totalBytes - 13);

            return totalBytes - 13;
        }

        public void ExtractFilesFromRaw(List<FileInfo> files)
        {
            using FileStream srcFileStream = new FileStream("F:\\MoonTorrent\\result", FileMode.Open);
            using BinaryReader srcBinaryReader = new BinaryReader(srcFileStream);

            foreach (FileInfo file in files)
            {
                using FileStream resultStream = new FileStream($"F:\\MoonTorrent\\RHCP-Unlimited Love\\{file.Name}", FileMode.Create);
                using BinaryWriter resultsWriter = new BinaryWriter(resultStream);
                var data = srcBinaryReader.ReadBytes(file.Size);
                resultsWriter.Write(data);
            }
        }

        public void FindRepeats()
        {
            int length = 30000;
            using FileStream fileStream1 = new FileStream("F:\\MoonTorrent\\01 - Black Summer.mp3", FileMode.Open);
            using BinaryReader binaryReader1 = new BinaryReader(fileStream1);
            using FileStream fileStream2 = new FileStream("F:\\MoonTorrent\\black.mp3", FileMode.Open);
            using BinaryReader binaryReader2 = new BinaryReader(fileStream2);

            var data1 = binaryReader1.ReadBytes(length);
            var data2 = binaryReader2.ReadBytes(length);

            for (int i = 0; i <  length; i++)
            {
                if (data1[i] != data2[i])
                {
                    Console.WriteLine($"False : {i}");
                    return;
                }
            }

            Console.WriteLine("True");
        }
    }
}
