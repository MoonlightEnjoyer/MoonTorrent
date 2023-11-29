using TorrentCore;

TorrentFileParser parser = new TorrentFileParser();

string infoHash = parser.CalculateInfoHash("F:\\MoonTorrent\\Red Hot Chili Peppers - Unlimited Love - 2022.torrent");

Console.WriteLine(infoHash);
