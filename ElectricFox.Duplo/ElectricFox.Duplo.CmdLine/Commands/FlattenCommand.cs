using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace ElectricFox.Duplo.CmdLine.Commands
{
    [Command("flatten", Description = "Move all files from all subdirectories to a new directory")]
    public class FlattenCommand : ICommand
    {
        [CommandParameter(0, Name = "source", Description = "Source directory", IsRequired = true)]
        public string? Source { get; set; }

        [CommandParameter(1, Name = "destination", Description = "Destination directory", IsRequired = true)]
        public string? Destination { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (Source is null || Destination is null)
            {
                throw new InvalidOperationException("Neither source or destination can be null");
            }

            var source = new DirectoryInfo(Source);
            MoveFiles(source, Destination);

            await console.Output.WriteLineAsync("Complete.");
        }

        private void MoveFiles(DirectoryInfo source, string dest)
        {
            foreach (var file in source.GetFiles())
            {
                var name = GetUniqueName(file.Name, dest);
                try
                {
                    file.MoveTo(Path.Combine(dest, name));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Cannot move {file.FullName}: {ex.Message}");
                }
            }

            foreach (var sub in source.GetDirectories())
            {
                MoveFiles(sub, dest);
            }
        }

        private string GetUniqueName(string orig, string dest)
        {
            var name = orig;
            int i = 1;
            while (File.Exists(Path.Combine(dest, name)))
            {
                name = $"{orig} ({i})";
                i++;
            }

            return name;
        }
    }
}
