using Docs.FileSystem;
using Docs.Utils;
using Handyman.Extensions;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Console = Docs.Utils.Console;

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
         private readonly IConsole _console;
         private readonly IFileSystem _fileSystem;

         public Worker() : this(new Console(), new FileSystem.FileSystem())
         {
         }

         public Worker(IConsole console, IFileSystem fileSystem)
         {
            _console = console;
            _fileSystem = fileSystem;
         }

         public void Execute(string path)
         {
            try
            {
               path = Path.GetFullPath(path);

               using (_console.CreateScope("Generating table of contents..."))
               {
                  var updatedFilesCounter = 0;
                  var elementParser = new DocsElementParser();
                  var headerParser = new HeaderParser();
                  var files = new MarkdownFileLocator(_fileSystem).GetFiles(path);

                  foreach (var file in files)
                  {
                     var lines = _fileSystem.ReadFile(file.FullPath);
                     var toc = elementParser.Parse(lines, "toc").SingleOrDefault();

                     if (toc == null)
                        continue;

                     updatedFilesCounter++;
                     _console.WriteInfo($"Updating {file.RelativePath}");

                     var headers = headerParser.Parse(lines.Skip(toc.ElementLine));

                     lines.RemoveRange(toc.ElementLine, toc.ElementLines);

                     var minLevel = headers.Min(x => x.Level);
                     headers.ForEach(x => x.Level -= minLevel);
                     var formatter = new LinkFormatter();
                     var tocContent = headers.Select(x =>
                        $"{Enumerable.Repeat("  ", x.Level).Join()}* {formatter.Format(x.Text)}");
                     lines.InsertRange(toc.ElementLine, new DocsElementWriter().Write(toc, tocContent));

                     _fileSystem.WriteFile(file.FullPath, lines);
                  }

                  _console.WriteInfo($"Updated {updatedFilesCounter} {(updatedFilesCounter == 1 ? "file" : "files")}.");
               }
            }
            catch (Exception exception)
            {
               _console.WriteError(exception.Message);
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