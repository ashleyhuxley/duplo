using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ElectricFox.Duplo.CmdLine.Extensions;

namespace ElectricFox.Duplo.CmdLine.Commands
{
    [Command("builddb", Description = "Build a database of file hashes from a path")]
    public class BuildDbCommand : ICommand
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

            data = new HashData();

            var dirInfo = new DirectoryInfo(Path);
            root = dirInfo.FullName;

            List<string> files = dirInfo.GetAllFilesInSubdirectories();

            int i = 0;
            using (var progress = new ProgressBar())
            {
                foreach (var file in files)
                {
                    var relativePath = file.Replace(root, "");
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Length < int.MaxValue)
                    {
                        using (var fileContent = File.OpenRead(file))
                        {
                            var hashValue = hash.ComputeHash(fileContent);
                            data.Add(new FileHash(hashValue, relativePath));
                        }
                    }

                    i++;
                    progress.Report((double)i / files.Count);
                }
            }

            data.Save(OutputPath);

            return ValueTask.CompletedTask;
        }
    }
}
