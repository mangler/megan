using System;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace search_files
{
    class Program
    {
        static void Main(string[] args)
        {
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

                        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => extentions.IndexOf(Path.GetExtension(f)) >= 0))
                        {
                            results[excludes.Any(c => Path.GetFileName(file).Contains(c)) ? Result.Ignored : Result.Found].Add(file);
                        }

                        foreach (var kvp in results.Where(c => c.Value.Any()))
                        {
                            Console.WriteLine($"Files {kvp.Key}");
                            foreach (var file in kvp.Value)
                            {
                                try
                                {
                                    using (var image = Image.Load(file))
                                    {
                                        Console.WriteLine($"\t{Path.GetFileName(file)} [{image.Width}x{image.Height}]");
                                    }
                                }
                                catch (SixLabors.ImageSharp.UnknownImageFormatException)
                                {
                                    Console.WriteLine($"\t{Path.GetFileName(file)}");
                                }
                            }
                            Console.WriteLine();
                        }
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

}
