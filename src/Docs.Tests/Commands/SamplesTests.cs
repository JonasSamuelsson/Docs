using Docs.Commands;
using Shouldly;
using Xunit;

namespace Docs.Tests.Commands
{
   public class SamplesTests
   {
      [Fact]
      public void ShouldImportWholeFileSamples()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\source.txt", new[] {"sample content"}},
               {@"x:\target.md", new[]
               {
                  "a",
                  "<!--<docs:sample src=\"source.txt\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs:sample src=\"source.txt\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs:sample>-->",
            "b"
         });
      }

      [Fact]
      public void ShouldImportNamedSamples()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\source.txt", new[]
               {
                  "x",
                  "<!--<docs:sample name=\"foo\">-->",
                  "sample content",
                  "<!--</docs:sample>-->",
                  "y"
               }},
               {@"x:\target.md", new[]
               {
                  "a",
                  "<!--<docs:sample src=\"source.txt#name=foo\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs:sample src=\"source.txt#name=foo\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs:sample>-->",
            "b"
         });
      }

      [Fact]
      public void ShouldImportLineSpanSamples()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\source.txt", new[]
               {
                  "x",
                  "sample content",
                  "y"
               }},
               {@"x:\target.md", new[]
               {
                  "a",
                  "<!--<docs:sample src=\"source.txt#lines=2-2\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs:sample src=\"source.txt#lines=2-2\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs:sample>-->",
            "b"
         });
      }

      [Fact]
      public void ShouldImportLineCountSamples()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\source.txt", new[]
               {
                  "x",
                  "sample content",
                  "y"
               }},
               {@"x:\target.md", new[]
               {
                  "a",
                  "<!--<docs:sample src=\"source.txt#lines=2:1\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs:sample src=\"source.txt#lines=2:1\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs:sample>-->",
            "b"
         });
      }
   }
}