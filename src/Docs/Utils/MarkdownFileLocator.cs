using Docs.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Docs.Utils
{
   public class MarkdownFileLocator
   {
      private readonly IFileSystem _fileSystem;

      public MarkdownFileLocator(IFileSystem fileSystem)
      {
         _fileSystem = fileSystem;
      }

      public IReadOnlyList<File> GetFiles(string path)
      {
         if (_fileSystem.FileExists(path))
         {
            var fullPath = Path.GetFullPath(path);
            return new[] { new File
            {
               FullPath = fullPath ,
               RelativePath = Path.GetFileName(fullPath)
            } };
         }

         if (_fileSystem.DirectoryExists(path))
         {
            var basePath = Path.GetFullPath(path);
            return _fileSystem
               .GetFiles(basePath, "*.md", SearchOption.AllDirectories)
               .Select(s => new File
               {
                  FullPath = s,
                  RelativePath = s.Substring(basePath.Length)
               })
               .ToList();
         }

         throw new AppException("path not found");
      }

      public class File
      {
         public string RelativePath { get; set; }
         public string FullPath { get; set; }
      }
   }
}