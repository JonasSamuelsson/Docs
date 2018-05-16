using Docs.FileSystem;
using Docs.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Console = Docs.Utils.Console;

namespace Docs.Commands
{
   public class Samples : CommandLineApplication
   {
      public static void Configure(CommandLineApplication command)
      {
         command.Description = "Import samples.";
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
         private readonly SettingsReader _settingsReader;

         public Worker() : this(new FileSystem.FileSystem())
         {
         }

         public Worker(IFileSystem fileSystem)
         {
            _fileSystem = fileSystem;
            _settingsReader = new SettingsReader(fileSystem);
         }

         public void Execute(string path)
         {
            var console = new Console();

            try
            {
               var srcPattern = @"^(?<uri>[^#]+)(#((id=(?<id>.+))|(lines=(?<from>\d+)((-(?<to>\d+))?)))?(language=(?<language>.+))?)?$";

               var elementParser = new DocsElementParser();
               var elementWriter = new DocsElementWriter();

               using (console.CreateScope("Importing samples..."))
               {
                  var updatedFilesCounter = 0;

                  foreach (var file in new MarkdownFileLocator(_fileSystem).GetFiles(path))
                  {
                     var fileContent = _fileSystem.ReadFile(file.FullPath);
                     var samples = elementParser.Parse(fileContent, "sample", "src");

                     if (!samples.Any())
                        continue;

                     console.WriteInfo($"Updating {file.RelativePath}");

                     foreach (var sample in samples.Reverse())
                     {
                        var src = Regex.Match(sample.Attributes["src"], srcPattern);

                        if (!src.Success)
                        {
                           console.WriteError($"  invalid src '{src}'.");
                           continue;
                        }

                        var uri = src.Groups["uri"].Value;

                        // todo - remote (http://...) uris

                        if (uri.StartsWith(@"$/") || uri.StartsWith(@"$\"))
                        {
                           var rootDirectory = _settingsReader.GetSamplesDirectory(file.FullPath);
                           uri = Path.Combine(rootDirectory, uri.Remove(0, 2));
                        }

                        if (!Path.IsPathRooted(uri))
                        {
                           var dir = Path.GetDirectoryName(file.FullPath);
                           uri = Path.Combine(dir, uri);
                        }

                        if (!_fileSystem.FileExists(uri))
                        {
                           console.WriteError($"  source not found ({uri}).");
                        }

                        var sampleContent = _fileSystem.ReadFile(uri);

                        var idGroup = src.Groups["id"];
                        if (idGroup.Success)
                        {
                           // todo error handling
                           var sampleElement = elementParser
                              .Parse(sampleContent, "sample", "id")
                              .Single(x => x.Attributes["id"].Equals(idGroup.Value, StringComparison.OrdinalIgnoreCase));

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
                           var toGroup = src.Groups["to"];
                           var count = toGroup.Success
                              ? int.Parse(toGroup.Value) - from
                              : 1;

                           sampleContent = sampleContent
                              .Skip(from)
                              .Take(count)
                              .ToList();
                        }

                        var languageGroup = src.Groups["language"];
                        var language = languageGroup.Success
                           ? languageGroup.Value
                           : GetLanguage(uri);

                        sampleContent.Insert(0, $"``` {language}".TrimEnd());
                        sampleContent.Add("```");

                        fileContent.RemoveRange(sample.ElementLine, sample.ElementLines);
                        fileContent.InsertRange(sample.ElementLine, elementWriter.Write(sample, sampleContent));
                     }

                     // todo only write file if its acually modified

                     _fileSystem.WriteFile(file.FullPath, fileContent);
                     updatedFilesCounter++;
                  }

                  console.WriteInfo($"Updated {updatedFilesCounter} {(updatedFilesCounter == 1 ? "file" : "files")}.");
               }
            }
            catch (Exception exception)
            {
               console.WriteError(exception.Message);
            }
         }

         private string GetLanguage(string uri)
         {
            var extension = Path.GetExtension(uri);
            return _settingsReader.TryGetSampleLanguage(uri, extension, out var language)
               ? language
               : Languages.TryGetValue(extension, out language)
                  ? language
                  : string.Empty;
         }
      }

      private static readonly Dictionary<string, string> Languages =
         new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
         {
            {".bat", "bat"},
            {".cmd", "cmd"},
            {".cs", "cs"},
            {".css", "css"},
            {".csx", "cs"},
            {".html", "html"},
            {".ini", "ini"},
            {".js", "js"},
            {".json", "json"},
            {".less", "less"},
            {".ps", "ps"},
            {".ts", "ts"},
            {".xml", "xml"}
         };
   }
}