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
        public void SendHandshake(IPEndPoint peer, HandshakePacket handshakePacket)
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

                if (bytesRead > 0 )
                {
                    //1. send interested message + wait choke/unchoke +
                    bool unchoke = this.SendInterested(stream);

                    if (unchoke)
                    {
                        Console.WriteLine($"{peer.Address} : unchoked");

                        //2. request file piece
                        this.RequestPiece(ref stream, 0);
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

        private void RequestPiece(ref NetworkStream stream, uint index)
        {
            Console.WriteLine("Requesting piece");

            byte[] message = {0, 0, 0, BinaryPrimitives.ReverseEndianness((byte)13), 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            BitConverter.GetBytes((ushort)index).CopyTo(message, 5);

            ushort blockSize = 16384;

            BitConverter.GetBytes(blockSize).CopyTo(message, 14);

            stream.Write(message);

            byte[] buffer = new byte[30000];

            int bytesRead = stream.Read(buffer);

            Console.WriteLine($"bytesRead = {bytesRead}");

            Console.WriteLine($"message id = {buffer[4]}");

        }
    }
}
