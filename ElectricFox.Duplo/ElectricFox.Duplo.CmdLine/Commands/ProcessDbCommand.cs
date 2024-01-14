using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace ElectricFox.Duplo.CmdLine.Commands
{
    [Command("process", Description = "Find duplicates in a json file")]
    public class ProcessDbCommand : ICommand
    {
        [CommandParameter(0, Name = "input", Description = "Input JSON file")]
        public string InputFile { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (!File.Exists(InputFile))
            {
                throw new FileNotFoundException(InputFile);
            }

            var data = HashData.LoadFrom(InputFile);

            var groups = data.Hashes
                .GroupBy(d => d.hash)
                .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                await console.Output.WriteLineAsync(group.Key);
                foreach (var val in group)
                {
                    await console.Output.WriteLineAsync("  " + val.relativePath);
                }

                await console.Output.WriteLineAsync();
            }
        }
    }
}
