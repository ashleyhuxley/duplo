﻿using CliFx;

namespace ElectricFox.Duplo.CmdLine
{
    internal static class Program
    {
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
    }
}