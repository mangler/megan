using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace search_files
{
    public class Options
    {
        public string PathToSearch { get; set; }
        public string ArchiveFolder { get; set; }
        public List<string> Extensions { get; set; }
        public List<string> Excludes { get; set; }
        public Dimensions Dimensions { get; set; }

        public override string ToString() => new StringBuilder().AppendLine($"Searching path: {PathToSearch}")
                                                                .AppendLine($"For extensions: {string.Join(' ', Extensions)}")
                                                                .AppendLine($"Excluding files containing the name: {string.Join(' ', Excludes)}")
                                                                .AppendLine($"Including dimensions: { Dimensions.Height}x{Dimensions.Width}")
                                                                .AppendLine()
                                                                .ToString();
    }

    public class Dimensions
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public static class Settings
    {
        internal static Options Options { get; }
        static Settings() => Options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("options.json"));
    }
}