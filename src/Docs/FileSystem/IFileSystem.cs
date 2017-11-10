using System.Collections.Generic;

namespace Docs.FileSystem
{
   public interface IFileSystem
   {
      bool DirectoryExists(string path);
      IReadOnlyList<string> GetFiles(string path, string pattern);
      bool FileExists(string path);
      List<string> ReadFile(string path);
      void WriteFile(string path, IEnumerable<string> lines);
   }
}