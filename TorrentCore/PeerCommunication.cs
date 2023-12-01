using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                //Console.WriteLine("Exceprion occured.");
            }
        }
    }
}
