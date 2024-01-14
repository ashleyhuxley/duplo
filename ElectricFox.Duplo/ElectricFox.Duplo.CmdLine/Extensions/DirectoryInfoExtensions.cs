namespace ElectricFox.Duplo.CmdLine.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static List<string> GetAllFilesInSubdirectories(this DirectoryInfo directory)
        {
            List<string> files = new();
            AddFiles(directory, files);
            return files;
        }

        private static void AddFiles(DirectoryInfo directory, List<string> files)
        {
            foreach (var file in directory.GetFiles().Select(f => f.FullName))
            {
                files.Add(file);
            }

            foreach (var sub in directory.GetDirectories())
            {
                AddFiles(sub, files);
            }
        }
    }
}
