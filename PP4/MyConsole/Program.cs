using Microsoft.EntityFrameworkCore;
using MyConsole.Context;
using MyConsole.Entities;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Iniciando aplicación ===");

        using (var db = new BooksContext())
        {
            string dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
            Directory.CreateDirectory(dataFolder);

            string csvPath = Path.Combine(dataFolder, "books.csv");

            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"❌ No se encontró el archivo CSV en: {csvPath}");
                return;
            }

            if (!db.Authors.Any())
            {
                Console.WriteLine("📚 La base de datos está vacía, se llenará con los datos del archivo CSV.");
                FillDatabaseFromCsv(db, csvPath);
                Console.WriteLine("✅ Base de datos llenada exitosamente.\n");
            }
            else
            {
                Console.WriteLine("📖 La base de datos ya contiene información.");
                GenerateTsvFiles(db, dataFolder);
                Console.WriteLine("✅ Archivos TSV generados exitosamente.\n");
            }
        }

        Console.WriteLine("=== Proceso completado ===");
    }

    static void FillDatabaseFromCsv(BooksContext db, string csvPath)
    {
        var lines = File.ReadAllLines(csvPath).Skip(1); 

        foreach (var line in lines)
        {
            var parts = ParseCsvLine(line);
            if (parts.Length < 3) continue;

            string authorName = parts[0].Trim();
            string titleName = parts[1].Trim();
            string[] tags = parts[2].Split('|', StringSplitOptions.TrimEntries);

            var author = db.Authors.FirstOrDefault(a => a.AuthorName == authorName);
            if (author == null)
            {
                author = new Author { AuthorName = authorName };
                db.Authors.Add(author);
                db.SaveChanges();
            }

            var title = new Title { TitleName = titleName, AuthorId = author.AuthorId };
            db.Titles.Add(title);
            db.SaveChanges();

            foreach (var tagName in tags)
            {
                var tag = db.Tags.FirstOrDefault(t => t.TagName == tagName);
                if (tag == null)
                {
                    tag = new Tag { TagName = tagName };
                    db.Tags.Add(tag);
                    db.SaveChanges();
                }

                db.TitleTags.Add(new TitleTag { TitleId = title.TitleId, TagId = tag.TagId });
            }

            db.SaveChanges();
        }
    }

    static void GenerateTsvFiles(BooksContext db, string outputDir)
    {
        var authors = db.Authors
            .OrderByDescending(a => a.AuthorName)
            .ToList();

        foreach (var author in authors)
        {
            char firstLetter = char.ToUpper(author.AuthorName[0]);
            string tsvPath = Path.Combine(outputDir, $"{firstLetter}.tsv");

            using var writer = new StreamWriter(tsvPath, append: false);
            writer.WriteLine("AuthorName\tTitleName\tTagName");

            var titles = db.Titles
                .Where(t => t.AuthorId == author.AuthorId)
                .OrderByDescending(t => t.TitleName)
                .ToList();

            foreach (var title in titles)
            {
                var tags = db.TitleTags
                    .Include(tt => tt.Tag)
                    .Where(tt => tt.TitleId == title.TitleId)
                    .OrderByDescending(tt => tt.Tag.TagName)
                    .ToList();

                foreach (var titleTag in tags)
                {
                    writer.WriteLine($"{author.AuthorName}\t{title.TitleName}\t{titleTag.Tag.TagName}");
                }
            }
        }
    }

    static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var value = "";

        foreach (var c in line)
        {
            if (c == '"' && inQuotes)
            {
                inQuotes = false;
            }
            else if (c == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(value);
                value = "";
            }
            else
            {
                value += c;
            }
        }
        result.Add(value);
        return result.ToArray();
    }
}
