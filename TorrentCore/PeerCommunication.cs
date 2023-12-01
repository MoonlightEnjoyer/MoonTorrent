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
                    //1. send interested message +
                    this.SendInterested(stream);


                    //2. wait for unchoke message +

                    Thread.Sleep(3000);



                    //3. request file piece
                    this.RequestPiece(stream, 0);
                }

                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                //Console.WriteLine("Exceprion occured.");
            }
        }

        private void SendInterested(NetworkStream stream)
        {
            byte[] message = [0, 0, 0, BinaryPrimitives.ReverseEndianness((byte)1), 2];

            stream.Write(message);
        }

        private void RequestPiece(NetworkStream stream, uint index)
        {
            byte[] message = new byte[13] {0, 0, 0, BinaryPrimitives.ReverseEndianness((byte)13), 6, 0, 0, 0, 0, 0, 0, 0, 0 };

            BitConverter.GetBytes(index).CopyTo(message, 5);


            stream.Write(message);
        }
    }
}
