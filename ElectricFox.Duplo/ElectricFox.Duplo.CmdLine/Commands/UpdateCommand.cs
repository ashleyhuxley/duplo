using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ElectricFox.Duplo.CmdLine.Extensions;

namespace ElectricFox.Duplo.CmdLine.Commands
{
    [Command("update", Description = "Update a database")]
    public class UpdateCommand : ICommand
    {
        [CommandParameter(0, Name = "path", Description = "Path containing source files")]
        public string Path { get; set; }

        [CommandParameter(1, Name = "output", Description = "Output file")]
        public string OutputPath { get; set; }

        private HashData? data;

        private string? root;

        private StreamHash hash = new StreamHash();

        public ValueTask ExecuteAsync(IConsole console)
        {
            if (!Directory.Exists(Path))
            {
                throw new DirectoryNotFoundException(Path);
            }

            console.Output.WriteLine("Loading file list...");

            data = HashData.LoadFrom(OutputPath);

            var dirInfo = new DirectoryInfo(Path);
            root = dirInfo.FullName;

            List<string> files = dirInfo.GetAllFilesInSubdirectories();

            // Regenerate hashes for new files
            console.Output.WriteLine("Regenerating hashes...");
            int i = 0;
            using (var progress = new ProgressBar())
            {
                foreach (var file in files)
                {
                    var relativePath = file.Replace(root, "");
                    if (!data.Hashes.Any(a => a.relativePath == relativePath))
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.Length < int.MaxValue)
                        {
                            using (var fileContent = File.OpenRead(file))
                            {
                                var hashValue = hash.ComputeHash(fileContent);
                                data.Add(new FileHash(hashValue, relativePath));
                            }
                        }
                    }

                    i++;
                    progress.Report((double)i / files.Count);
                }
            }

            // Remove nonexistant entries
            console.Output.WriteLine("Removing nonexistant entries...");
            List<FileHash> toRemove = new();
            foreach (var hash in data.Hashes)
            {
                var fullPath = root + hash.relativePath;
                if (!files.Contains(fullPath))
                {
                    toRemove.Add(hash);
                }
            }
            foreach (var hash in toRemove)
            {
                data.Remove(hash);
            }

            data.Save(OutputPath);

            return ValueTask.CompletedTask;
        }
    }
}
