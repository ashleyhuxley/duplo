using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace ElectricFox.Duplo.CmdLine.Commands
{
    [Command("compare", Description = "Scan a folder for duplicates in a db file")]
    public class CompareCommand : ICommand
    {
        [CommandParameter(0, Name = "source", Description = "Source db file")]
        public string Source { get; set; }

        [CommandParameter(1, Name = "dest", Description = "Comparison db file")]
        public string Destination { get; set; }

        [CommandParameter(2, Name = "sourcedir", Description = "Source directory")]
        public string SourceDir { get; set; }

        [CommandParameter(3, Name = "delete", Description = "Delete duplicates from source")]
        public bool Delete { get; set; }

        private HashData? sourceData;
        private HashData? destinationData;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            sourceData = HashData.LoadFrom(Source);
            destinationData = HashData.LoadFrom(Destination);

            foreach (var hash in sourceData.Hashes)
            {
                var dupe = destinationData.Hashes.FirstOrDefault(h => h.hash == hash.hash);
                if (dupe != null)
                {
                    await console.Output.WriteLineAsync(hash.relativePath);
                    await console.Output.WriteLineAsync(dupe.relativePath);
                    await console.Output.WriteLineAsync();

                    if (Delete)
                    {
                        File.Delete(SourceDir + hash.relativePath);
                    }
                }
            }

            await console.Output.WriteLineAsync("Complete.");
        }
    }
}
