using Docs.FileSystem;
using System;
using System.IO;
using System.Linq;

namespace Docs.Utils
{
   public class RootDirectoryResolver
   {
      private readonly IFileSystem _fileSystem;

      public RootDirectoryResolver(IFileSystem fileSystem)
      {
         _fileSystem = fileSystem;
      }

      public string GetRootDirectory(string path)
      {
         var fileName = ".docsconfig.";

         for (var dir = GetDirectory(path); dir != null; dir = Path.GetDirectoryName(dir))
         {
            var file = _fileSystem.GetFiles(dir, fileName, SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (file == null)
               continue;

            var filePath = Path.Combine(dir, fileName);

            foreach (var line in _fileSystem.ReadFile(filePath).Select(x => x.ToLower().Trim()))
            {
               if (line == string.Empty || line.StartsWith("#"))
                  continue;

               var items = line
                  .Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)
                  .Select(x => x.Trim())
                  .ToArray();

               if (items[0] != "root")
                  continue;

               return items.Length == 2 && bool.TryParse(items[1], out var result) && result
                  ? dir
                  : throw new AppException("Invalid config.");
            }
         }

         throw new AppException("Root directory not found.");
      }

      private string GetDirectory(string path)
      {
         if (_fileSystem.FileExists(path))
            return Path.GetDirectoryName(path);

         if (_fileSystem.DirectoryExists(path))
            return path;

         throw new FileNotFoundException();
      }
   }
}