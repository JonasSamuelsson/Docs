using Docs.FileSystem;
using System.Collections.Generic;
using System.IO;

namespace Docs.Utils
{
   public class MarkdownFileLocator
   {
      private readonly IFileSystem _fileSystem;

      public MarkdownFileLocator(IFileSystem fileSystem)
      {
         _fileSystem = fileSystem;
      }

      public IReadOnlyList<string> GetFiles(string path)
      {
         return _fileSystem.FileExists(path)
            ? new[] { Path.GetFullPath(path) }
            : _fileSystem.DirectoryExists(path)
               ? _fileSystem.GetFiles(path, "*.md")
               : throw new AppException("path not found");
      }
   }
}