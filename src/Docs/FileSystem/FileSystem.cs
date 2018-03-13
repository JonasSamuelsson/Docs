using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Docs.FileSystem
{
   public class FileSystem : IFileSystem
   {
      public bool DirectoryExists(string path)
      {
         return Directory.Exists(path);
      }

      public IReadOnlyList<string> GetFiles(string path, string pattern, SearchOption searchOption)
      {
         return Directory.GetFiles(path, pattern, searchOption);
      }

      public bool FileExists(string path)
      {
         return File.Exists(path);
      }

      public List<string> ReadFile(string path)
      {
         return File.ReadAllLines(path).ToList();
      }

      public void WriteFile(string path, IEnumerable<string> lines)
      {
         File.WriteAllLines(path, lines);
      }
   }
}