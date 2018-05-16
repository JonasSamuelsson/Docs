using Docs.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Docs.Tests
{

   public class TestFileSystem : IFileSystem
   {
      public Dictionary<string, IEnumerable<string>> Files { get; set; } = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);

      public bool DirectoryExists(string path)
      {
         foreach (var file in Files.Keys)
         {
            var dir = Path.GetDirectoryName(file);
            if (dir.Equals(path, StringComparison.OrdinalIgnoreCase))
               return true;
         }

         return false;
      }

      public IReadOnlyList<string> GetFiles(string path, string pattern, SearchOption searchOption)
      {
         path = path.TrimEnd('\\') + "\\";
         pattern = Regex.Escape(pattern).Replace("\\*", ".*");

         var files = Files.Keys.Where(x => x.StartsWith(path));

         files = files.Where(x => Regex.IsMatch(Path.GetFileName(x), $"^{pattern}$", RegexOptions.IgnoreCase));

         if (searchOption == SearchOption.TopDirectoryOnly)
         {
            path = path.TrimEnd('\\');
            if (path.EndsWith(":")) path += @"\";
            files = files.Where(x => Path.GetDirectoryName(x).Equals(path, StringComparison.OrdinalIgnoreCase));
         }

         return files.ToList();
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