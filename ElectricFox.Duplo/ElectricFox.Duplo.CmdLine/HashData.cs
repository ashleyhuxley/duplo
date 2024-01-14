using System.Text.Json;

namespace ElectricFox.Duplo.CmdLine
{
    internal class HashData
    {
        private readonly List<FileHash> hashes;

        public IReadOnlyList<FileHash> Hashes => hashes;

        public void Add(FileHash hash)
        {
            hashes.Add(hash);
        }

        public void Remove(FileHash hash)
        {
            hashes.Remove(hash);
        }

        public HashData()
        {
            hashes = new List<FileHash>();
        }

        private HashData(FileHash[] hashes)
        {
            this.hashes = hashes.ToList();
        }

        public void Save(string filename)
        {
            var json = JsonSerializer.Serialize(hashes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }

        public static HashData LoadFrom(string filename)
        {
            using (var file = File.OpenRead(filename))
            {
                var h = JsonSerializer.Deserialize<FileHash[]>(file);
                if (h is null)
                {
                    return new HashData();
                }

                return new HashData(h);
            }
        }
    }

    internal record class FileHash
        (string hash,
        string relativePath)
    { }
}
