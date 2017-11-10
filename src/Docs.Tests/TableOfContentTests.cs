using Docs.Commands;
using Docs.FileSystem;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Docs.Tests
{
   public class TableOfContentTests
   {
      [Fact]
      public void ShouldGenerateToc()
      {
         var content = new[]
         {
            "a",
            "# 1" ,
            "b",
            "<!--<docs:toc/>-->",
            "c",
            "# 2",
            "d",
            "## 2.1",
            "e",
            "## 2.2",
            "f",
            "# 3",
            "g"
         };

         var fs = new TestFileSystem { Files = { { @"x:\toc.md", content } } };

         new TableOfContent.Worker(fs).Execute(@"x:\toc.md");

         fs.Files[@"x:\toc.md"].ShouldBe(new[]
         {
            "a",
            "# 1",
            "b",
            "<!--<docs:toc>-->",
            "* [2](#2)",
            "  * [2.1](#2.1)",
            "  * [2.2](#2.2)",
            "* [3](#3)",
            "<!--</docs:toc>-->",
            "c",
            "# 2",
            "d",
            "## 2.1",
            "e",
            "## 2.2",
            "f",
            "# 3",
            "g"
         });
      }
   }

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