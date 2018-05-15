using Docs.Commands;
using Shouldly;
using Xunit;

namespace Docs.Tests.Commands
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
            "[//]: # (<docs-toc/>)",
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
            "[//]: # (<docs-toc>)",
            "* [2](#2)",
            "  * [2.1](#21)",
            "  * [2.2](#22)",
            "* [3](#3)",
            "[//]: # (</docs-toc>)",
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
}