using Docs.FileSystem;
using Docs.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using System.Linq;

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
            var srcPattern = "^(?<uri>.+)(#(?<section>.+))+$";
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
                  var src = sample.Attributes["src"];

                  // todo - handle rooted ($/foo/bar.ext) & remote (http://...) uris
                  // todo handle named samples
                  // todo handle lines samples

                  if (!Path.IsPathRooted(src))
                  {
                     var dir = Path.GetDirectoryName(file);
                     src = Path.Combine(dir, src);
                  }

                  if (!_fileSystem.FileExists(src))
                     // todo better error message
                     throw new AppException($"File not found ({src}).");

                  var sampleContent = _fileSystem.ReadFile(src);
                  sampleContent.Insert(0, "```");
                  sampleContent.Add("```");

                  fileContent.RemoveRange(sample.Line, sample.Lines);
                  fileContent.InsertRange(sample.Line, elementWriter.Write(sample, sampleContent));
               }

               // todo only write file if its acually modified

               _fileSystem.WriteFile(file, fileContent);

               // todo write feedback
            }
         }
      }
   }
}