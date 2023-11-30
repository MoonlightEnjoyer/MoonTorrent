using TorrentCore;

string torrentFilePath = "F:\\MoonTorrent\\Red Hot Chili Peppers - Unlimited Love - 2022.torrent";

TorrentFile torrentFile = new TorrentFile(torrentFilePath, 6889);

Console.WriteLine(torrentFile.InfoHash);
Console.WriteLine(torrentFile.InfoHashUrlEncoded);

Console.WriteLine(torrentFile.Left);