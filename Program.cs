using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace search_files
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var path = GetPath();
            List<string> extentions;
            if (!string.IsNullOrEmpty(path))
            {
                extentions = GetExtensions();
                if (extentions.Any())
                {
                    var excludes = GetExclusions();
                    Console.Clear();
                    Console.WriteLine($"Searching path: {path}");
                    Console.WriteLine($"For extensions: {string.Join(' ', extentions)}");
                    Console.WriteLine($"Excluding files containing the name: {string.Join(' ', excludes)}{Environment.NewLine}");
                    try
                    {
                        var results = new Dictionary<string, List<string>>
                        {
                            {Result.Ignored, new List<string>()},
                            {Result.Found, new List<string>()},
                        };

                        var toProcess = new List<Info>();
                        var notImage = new List<string>();

                        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => extentions.IndexOf(Path.GetExtension(f)) >= 0))
                        {
                            results[excludes.Any(c => Path.GetFileName(file.ToLower()).Contains(c.ToLower())) ? Result.Ignored : Result.Found].Add(file);
                        }

                        foreach (var kvp in results.Where(c => c.Value.Any()))
                        {
                            foreach (var file in kvp.Value)
                            {
                                if (kvp.Key == Result.Ignored)
                                {
                                    Console.WriteLine($"\tIgnored: {Path.GetFileName(file)}");
                                }
                                else
                                {
                                    try
                                    {
                                        using (var image = SixLabors.ImageSharp.Image.Load(file))
                                        {
                                            toProcess.Add(new Info
                                            {
                                                Path = file,
                                                Height = image.Height,
                                                Width = image.Width

                                            });
                                            Console.WriteLine($"\tProcessed: {Path.GetFileName(file)} [{image.Width}x{image.Height}]");
                                        }
                                    }
                                    catch (SixLabors.ImageSharp.UnknownImageFormatException)
                                    {
                                        Console.WriteLine($"\tNot an image: {Path.GetFileName(file)}");
                                        notImage.Add(Path.GetFileName(file));
                                    }
                                }

                            }
                        }

                        WriteToExcel(toProcess, results[Result.Ignored], notImage);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not search: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Could not get extensions.");
                }
            }
            else
            {
                Console.WriteLine("Could not get a valid path");
            }
            Console.WriteLine();
            Console.WriteLine("Done! Press any key to exit...");
            Console.ReadKey();
        }
        private static void WriteToExcel(List<Info> found, List<string> ignored, List<string> notimage)
        {

            using (var package = new ExcelPackage(new System.IO.FileInfo($"results_{DateTime.Now.ToFileTimeUtc()}.xlsx")))
            {
                var ignoreSheet = package.Workbook.Worksheets.Add("Ignored from exclusion filter");
                for (int i = 0; i < ignored.Count; i++)
                {
                    ignoreSheet.Cells[$"A{i + 1}"].Value = Path.GetFileName(ignored[i]);
                }
                ignoreSheet.Column(1).AutoFit();
                ignoreSheet.InsertRow(1, 1);
                ignoreSheet.Cells["A1"].Style.Font.Bold = true;
                ignoreSheet.Cells["A1"].Value = "Ignored";

                var notImageSheet = package.Workbook.Worksheets.Add("Found but not an image");
                for (int i = 0; i < notimage.Count; i++)
                {
                    notImageSheet.Cells[$"A{i + 1}"].Value = Path.GetFileName(notimage[i]);
                }
                notImageSheet.Column(1).AutoFit();
                notImageSheet.InsertRow(1, 1);
                notImageSheet.Cells["A1"].Style.Font.Bold = true;
                notImageSheet.Cells["A1"].Value = "Not an image";

                var foundSheet = package.Workbook.Worksheets.Add("Found");
                for (int i = 0; i < found.Count; i++)
                {
                    foundSheet.Cells[i + 1, 1].Value = Path.GetFileName(found[i].Path);
                    foundSheet.Cells[i + 1, 2].Value = Path.GetDirectoryName(found[i].Path);
                    foundSheet.Cells[i + 1, 3].Value = found[i].Height;
                    foundSheet.Cells[i + 1, 4].Value = found[i].Width;
                }
                foundSheet.Column(1).AutoFit();
                foundSheet.Column(2).AutoFit();
                foundSheet.Column(3).AutoFit();
                foundSheet.Column(4).AutoFit();
                foundSheet.InsertRow(1, 1);
                foundSheet.Cells["A1"].Style.Font.Bold = true;
                foundSheet.Cells["A1"].Value = "File";
                foundSheet.Cells["B1"].Style.Font.Bold = true;
                foundSheet.Cells["B1"].Value = "Path";
                foundSheet.Cells["C1"].Style.Font.Bold = true;
                foundSheet.Cells["C1"].Value = "Height";
                foundSheet.Cells["D1"].Style.Font.Bold = true;
                foundSheet.Cells["D1"].Value = "Height";
                foundSheet.Cells["A1:D1"].AutoFilter = true;
                
                package.Save();
            }
        }

        private static List<string> GetExclusions()
        {
            try
            {
                var exclusions = new List<string>();
                var stop = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter x when done");
                    Console.Write($"Enter exclusion pattern >> {exclusions.Count} <<: ");
                    var candidate = Console.ReadLine();
                    switch (candidate.ToLower())
                    {
                        case "x":
                            stop = true;
                            break;
                        default:
                            {
                                exclusions.Add(candidate);
                                break;
                            }
                    }
                } while (!stop);

                return exclusions;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }

        static List<string> GetExtensions()
        {
            try
            {
                var extentions = new List<string>();
                var stop = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter x when done");
                    Console.Write($"Enter an extension to search [i.e. .txt, .jpg] >> {extentions.Count} <<: ");
                    var candidate = Console.ReadLine();
                    switch (candidate.ToLower())
                    {
                        case "x":
                            stop = true;
                            break;
                        default:
                            {
                                if (candidate.StartsWith("."))
                                {
                                    extentions.Add(candidate);
                                }
                                break;
                            }
                    }
                } while (!stop);

                return extentions;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }

        static string GetPath()
        {
            try
            {
                var path = string.Empty;
                var stop = false;
                do
                {
                    Console.Clear();
                    Console.Write("Enter a valid path: ");
                    path = Console.ReadLine();
                    if (Directory.Exists(path))
                    {
                        stop = true;
                    }
                } while (!stop);

                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
    internal class Result
    {
        public const string Found = "Found";
        public const string Ignored = "Ignored";
    }

    internal class Info
    {
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

}
