using Docs.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Docs.Utils
{
   public class SettingsReader
   {
      private readonly IFileSystem _fileSystem;

      public SettingsReader(IFileSystem fileSystem)
      {
         _fileSystem = fileSystem;
      }

      public string GetSamplesDirectory(string path)
      {
         if (!TryGetSettings(path, out var settings))
            throw new AppException("Could not find .docs file.");

         if (!settings.Values.TryGetValue("samples.dir", out var dir))
            throw new AppException(".docs doesn't contain setting 'sample.dir'.");

         if (!Path.IsPathRooted(dir))
            dir = Path.Combine(settings.Directory, dir);

         return Path.GetFullPath(dir);
      }

      public bool TryGetSampleLanguage(string path, string extension, out string language)
      {
         language = null;
         var key = $"samples.languages{extension}".ToLower();
         return TryGetSettings(path, out var settings)
                && settings.Values.TryGetValue(key, out language);
      }

      private bool TryGetSettings(string path, out Settings settings)
      {
         var fileName = ".docs.";

         for (var dir = GetDirectory(path); dir != null; dir = Path.GetDirectoryName(dir))
         {
            var file = _fileSystem.GetFiles(dir, fileName, SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (file == null)
               continue;

            var filePath = Path.Combine(dir, fileName);
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in _fileSystem.ReadFile(filePath).Select(x => x.ToLower().Trim()))
            {
               if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                  continue;

               var items = line
                  .Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)
                  .Select(x => x.Trim())
                  .ToArray();

               dictionary.Add(items[0], items[1]);
            }

            settings = new Settings
            {
               Directory = dir,
               Path = filePath,
               Values = dictionary
            };
            return true;
         }

         settings = null;
         return false;
      }

      private string GetDirectory(string path)
      {
         if (_fileSystem.FileExists(path))
            return Path.GetDirectoryName(path);

         if (_fileSystem.DirectoryExists(path))
            return path;

         throw new FileNotFoundException();
      }

      private class Settings
      {
         public string Path { get; set; }
         public string Directory { get; set; }
         public Dictionary<string, string> Values { get; set; }
      }
   }
}