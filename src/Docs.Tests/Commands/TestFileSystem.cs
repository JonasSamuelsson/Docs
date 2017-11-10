using System;
using System.Collections.Generic;
using System.Linq;
using Docs.FileSystem;

namespace Docs.Tests.Commands
{
   public class TestFileSystem : IFileSystem
   {
      public Dictionary<string, IEnumerable<string>> Files { get; set; } = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);

      public bool DirectoryExists(string path)
      {
         throw new System.NotImplementedException();
      }

      public IReadOnlyList<string> GetFiles(string path, string pattern)
      {
         throw new System.NotImplementedException();
      }

      public bool FileExists(string path)
      {
         return Files.ContainsKey(path);
      }

      public List<string> ReadFile(string path)
      {
         return Files[path].ToList();
      }

      public void WriteFile(string path, IEnumerable<string> lines)
      {
         Files[path] = lines;
      }
   }
}