using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System;
using OfficeOpenXml.Style;
using System.Linq;
using System.Drawing;

namespace search_files
{
    public static class Utils
    {
        static string ArchiveDir;
        static Utils()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ArchiveDir = Directory.CreateDirectory($"{Settings.Options.PathToSearch}/{Settings.Options.ArchiveFolder}/{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}").FullName;
        }
        public static void WriteToExcelAndArchive(List<Info> files)
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo($"{ArchiveDir}/results.xlsx")))
            {
                var all = package.Workbook.Worksheets.Add("Index");
                for (int i = 0; i < files.Count; i++)
                {
                    all.Cells[i + 1, 1].Value = Path.GetFileName(files[i].Path);
                    all.Cells[i + 1, 2].Value = files[i].Height;
                    all.Cells[i + 1, 3].Value = files[i].Width;
                    all.Cells[i + 1, 4].Value = Path.GetDirectoryName(files[i].Path);

                    if ((!Settings.Options.Excludes.Any(c=> Path.GetFileName(files[i].Path.ToLower()).Contains(c.ToLower()))) && (files[i].Height < Settings.Options.Dimensions.Height || files[i].Width < Settings.Options.Dimensions.Width))
                    {
                        all.Row(i + 1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                        all.Row(i + 1).Style.Fill.BackgroundColor.SetColor(Color.Orange);
                        Archive(files[i].Path);
                    }

                }

                all.Column(1).AutoFit();
                all.Column(2).AutoFit();
                all.Column(3).AutoFit();
                all.Column(4).AutoFit();

                all.InsertRow(1, 1);
                all.Cells["A1"].Style.Font.Bold = true;
                all.Cells["A1"].Value = "File";
                all.Cells["B1"].Style.Font.Bold = true;
                all.Cells["B1"].Value = "Height";
                all.Cells["C1"].Style.Font.Bold = true;
                all.Cells["C1"].Value = "Width";
                all.Cells["D1"].Style.Font.Bold = true;
                all.Cells["D1"].Value = "Path";
                all.Cells["A1:D1"].AutoFilter = true;

                package.Save();
            }
        }

        static void Archive(string file) => File.Move(file, $"{ArchiveDir}/{Path.GetFileName(file)}", true);
    }
}

