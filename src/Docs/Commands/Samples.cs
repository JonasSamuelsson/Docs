using System;
using Docs.FileSystem;
using Docs.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Docs.Commands
{
   public class Samples : CommandLineApplication
   {
      public static void Configure(CommandLineApplication command)
      {
         command.Description = "Imports sample code.";
         command.HelpOption("-?|-h|--help").ShowInHelpText = false;

         var args = new
         {
            path = command.Argument("path", "Target file or folder.")
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
            var srcPattern = @"^(?<uri>[^#]+)(#((name=(?<name>.+))|(lines=(?<from>\d+)((-(?<to>\d+))|(:(?<count>\d+))))))?$";

            var elementParser = new DocsElementParser();
            var elementWriter = new DocsElementWriter();

            foreach (var file in new MarkdownFileLocator(_fileSystem).GetFiles(path))
            {
               var fileContent = _fileSystem.ReadFile(file);
               var samples = elementParser.Parse(fileContent, "sample", "src");

               if (!samples.Any())
                  continue;

               foreach (var sample in samples.Reverse())
               {
                  var src = Regex.Match(sample.Attributes["src"], srcPattern);

                  if (!src.Success)
                     // todo better error message
                     throw new AppException($"Invalid src '{src}'.");

                  var uri = src.Groups["uri"].Value;

                  // todo - handle rooted ($/foo/bar.ext) & remote (http://...) uris

                  if (!Path.IsPathRooted(uri))
                  {
                     var dir = Path.GetDirectoryName(file);
                     uri = Path.Combine(dir, uri);
                  }

                  if (!_fileSystem.FileExists(uri))
                     // todo better error message
                     throw new AppException($"File not found ({uri}).");

                  var sampleContent = _fileSystem.ReadFile(uri);

                  var nameGroup = src.Groups["name"];
                  if (nameGroup.Success)
                  {
                     // todo error handling
                     var sampleElement = elementParser
                        .Parse(sampleContent, "sample", "name")
                        .Single(x => x.Attributes["name"].Equals(nameGroup.Value, StringComparison.OrdinalIgnoreCase));

                     sampleContent = sampleContent
                        .Skip(sampleElement.ContentLine)
                        .Take(sampleElement.ContentLines)
                        .ToList();
                  }

                  var fromGroup = src.Groups["from"];
                  if (fromGroup.Success)
                  {
                     // todo error handling, count/to to big/small
                     var from = int.Parse(fromGroup.Value) - 1;
                     var countGroup = src.Groups["count"];
                     var count = countGroup.Success
                        ? int.Parse(countGroup.Value)
                        : int.Parse(src.Groups["to"].Value) - from;

                     sampleContent = sampleContent
                        .Skip(from)
                        .Take(count)
                        .ToList();
                  }

                  sampleContent.Insert(0, "```");
                  sampleContent.Add("```");

                  fileContent.RemoveRange(sample.ElementLine, sample.ElementLines);
                  fileContent.InsertRange(sample.ElementLine, elementWriter.Write(sample, sampleContent));
               }

               // todo only write file if its acually modified

               _fileSystem.WriteFile(file, fileContent);

               // todo write feedback
            }
         }
      }
   }
}