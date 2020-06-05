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

            var files = Directory.GetFiles(Settings.Options.PathToSearch, "*.*", SearchOption.AllDirectories)
                                  .Where(f => Settings.Options.Extensions.IndexOf(Path.GetExtension(f.ToLower())) >= 0)
                                  .Select(c => new Info { Path = c })
                                  .ToList();

            foreach (var file in files)
            {
                try
                {
                    using (var image = SixLabors.ImageSharp.Image.Load(file.Path))
                    {
                        file.Width = image.Width;
                        file.Height = image.Height;
                    }
                }
                catch (SixLabors.ImageSharp.UnknownImageFormatException) { }
            }

            Utils.WriteToExcelAndArchive(files);

            spinner.Stop();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
