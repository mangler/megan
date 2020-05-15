using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
                    Console.WriteLine($"Searching {path} for {string.Join(',', extentions)}");
                    try
                    {
                        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => extentions.IndexOf(Path.GetExtension(f)) >= 0))
                        {
                            Console.WriteLine(file);
                        }
                        Console.WriteLine("Done! Press any key to exit...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not search: {ex.Message}");
                    }
                    Console.ReadLine();
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
        }

        static List<string> GetExtensions()
        {
            try
            {
                var extentions = new List<string>();
                var stop = false;
                Console.WriteLine("Enter x when done");
                do
                {
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
}
