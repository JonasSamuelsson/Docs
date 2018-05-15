using Docs.FileSystem;
using Docs.Utils;
using Handyman.Extensions;
using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace Docs.Commands
{
   public class TableOfContent : CommandLineApplication
   {
      public static void Configure(CommandLineApplication command)
      {
         command.Description = "Generates table of content from headers.";
         command.HelpOption("-?|-h|--help").ShowInHelpText = false;

         var args = new
         {
            path = command.Argument("path", "Target file or filder.")
         };

         command.OnExecute(() =>
         {
            var path = args.path.Value;
            new Worker().Execute(path);
            return 0;
         });
      }

      public class Worker
      {
         private readonly IFileSystem _fileSystem;

         public Worker() : this(new FileSystem.FileSystem())
         {
         }

         public Worker(IFileSystem fileSystem)
         {
            _fileSystem = fileSystem;
         }

         public void Execute(string path)
         {
            var elementParser = new DocsElementParser();
            var headerParser = new HeaderParser();
            var files = new MarkdownFileLocator(_fileSystem).GetFiles(path);

            foreach (var file in files)
            {
               var lines = _fileSystem.ReadFile(file);
               var toc = elementParser.Parse(lines, "toc").SingleOrDefault();

               if (toc == null)
                  continue;

               var headers = headerParser.Parse(lines.Skip(toc.ElementLine));

               if (!headers.Any())
                  continue;

               lines.RemoveRange(toc.ElementLine, toc.ElementLines);

               var minLevel = headers.Min(x => x.Level);
               headers.ForEach(x => x.Level -= minLevel);
               var formatter = new LinkFormatter();
               var tocContent = headers.Select(x => $"{Enumerable.Repeat("  ", x.Level).Join()}* {formatter.Format(x.Text)}");
               lines.InsertRange(toc.ElementLine, new DocsElementWriter().Write(toc, tocContent));

               _fileSystem.WriteFile(file, lines);
            }
         }
      }

      public class LinkFormatter
      {
         private readonly List<string> _history = new List<string>();

         public string Format(string text)
         {
            var chars = text
               .ToLower()
               .Select(c => c)
               .Where(c => c == ' ' || c == '-' || char.IsLetterOrDigit(c));

            var link = string.Join(string.Empty, chars).Replace(" ", "-");

            _history.Add(link);
            var count = _history.Count(x => x == link);
            if (count > 1)
            {
               link += $"-{count - 1}";
            }

            return $"[{text}](#{link})";
         }
      }
   }
}