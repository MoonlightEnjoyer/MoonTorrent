using TorrentCore;

string torrentFilePath = "F:\\MoonTorrent\\Red Hot Chili Peppers - Unlimited Love - 2022.torrent";

TorrentFile torrentFile = new TorrentFile(torrentFilePath, 6889);

Console.WriteLine(torrentFile.InfoHashString);
Console.WriteLine(torrentFile.InfoHashUrlEncoded);

Console.WriteLine(torrentFile.Left);

TrackerCommunication trackerCommunication = new TrackerCommunication();

var peersList = trackerCommunication.GetPeers(torrentFile);

HandshakePacket handshakePacket = new HandshakePacket(torrentFile.InfoHash, torrentFile.PeerId);

PeerCommunication peerCommunication = new PeerCommunication();

foreach (var peer in peersList)
{
    peerCommunication.SendHandshake(peer, handshakePacket);
}