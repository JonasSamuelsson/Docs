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
                  "<!--<docs-sample src=\"source.txt\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs-sample src=\"source.txt\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs-sample>-->",
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
                  "<!--<docs-sample id=\"foo\">-->",
                  "sample content",
                  "<!--</docs-sample>-->",
                  "y"
               }},
               {@"x:\target.md", new[]
               {
                  "a",
                  "<!--<docs-sample src=\"source.txt#id=foo\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs-sample src=\"source.txt#id=foo\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs-sample>-->",
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
                  "<!--<docs-sample src=\"source.txt#lines=2-2\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs-sample src=\"source.txt#lines=2-2\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs-sample>-->",
            "b"
         });
      }

      [Fact]
      public void ShouldImportSingleLineSamples()
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
                  "<!--<docs-sample src=\"source.txt#lines=2\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "a",
            "<!--<docs-sample src=\"source.txt#lines=2\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs-sample>-->",
            "b"
         });
      }

      [Fact]
      public void ShouldImportSampleWithRootedSrcPath()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\root\.docs.", new[] {"samples.dir:."}},
               {@"x:\root\sample.txt", new[] {"success"}},
               {@"x:\root\child\target.md", new[] {"<!--<docs-sample src=\"$\\sample.txt\"/>-->"}}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\root\child\target.md");

         fs.Files[@"x:\root\child\target.md"].ShouldBe(new[]
         {
            "<!--<docs-sample src=\"$\\sample.txt\">-->",
            "```",
            "success",
            "```",
            "<!--</docs-sample>-->"
         });
      }

      [Fact]
      public void ShouldDeriveSampleLanguageFromExtension()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\sample.cs", new[] {"success"}},
               {@"x:\target.md", new[] {"<!--<docs-sample src=\"sample.cs\"/>-->"}}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "<!--<docs-sample src=\"sample.cs\">-->",
            "``` cs",
            "success",
            "```",
            "<!--</docs-sample>-->"
         });
      }

      [Fact]
      public void ShouldDeriveSampleLanguageFromExtensionAndSettings()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\.docs.", new[] {"samples.languages.cs:override"}},
               {@"x:\sample.cs", new[] {"success"}},
               {@"x:\target.md", new[] {"<!--<docs-sample src=\"sample.cs\"/>-->"}}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "<!--<docs-sample src=\"sample.cs\">-->",
            "``` override",
            "success",
            "```",
            "<!--</docs-sample>-->"
         });
      }

      [Fact]
      public void ShouldAddSpecifiedLanguageToSample()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\sample.txt", new[] {"success"}},
               {@"x:\target.md", new[] {"<!--<docs-sample src=\"sample.txt#language=foobar\"/>-->"}}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target.md");

         fs.Files[@"x:\target.md"].ShouldBe(new[]
         {
            "<!--<docs-sample src=\"sample.txt#language=foobar\">-->",
            "``` foobar",
            "success",
            "```",
            "<!--</docs-sample>-->"
         });
      }
   }
}