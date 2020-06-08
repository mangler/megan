using System;
using System.IO;
using System.Linq;

namespace search_files
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(Settings.Options);

            var spinner = new Spinner(10, 10);

            spinner.Start();

            var files =
             new DirectoryInfo(Settings.Options.PathToSearch)
                  .EnumerateDirectories()
                  .SelectMany(di => di.EnumerateFiles("*.*", SearchOption.AllDirectories)
                  .Where(f => Settings.Options.Extensions.Any(g => g.Equals(f.Extension, StringComparison.CurrentCultureIgnoreCase)))
                  .Where(p => !Path.GetDirectoryName(p.DirectoryName).Contains(Settings.Options.ArchiveFolder, StringComparison.CurrentCultureIgnoreCase))
                  .Select(c => new Info { Path = c.FullName }))
                  .ToList();

            foreach (var file in files)
            {
                try
                {
                    using (var stream = File.Open(file.Path, FileMode.Open))
                    using (var image = System.Drawing.Image.FromStream(stream, false, false))
                    {
                        file.Width = image.Width;
                        file.Height = image.Height;
                    }
                }
                catch (ArgumentException) { }

            }

            Utils.WriteToExcelAndArchive(files);

            spinner.Stop();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
