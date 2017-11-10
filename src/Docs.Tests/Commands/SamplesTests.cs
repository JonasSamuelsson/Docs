using Docs.Commands;
using Shouldly;
using Xunit;

namespace Docs.Tests.Commands
{
   public class SamplesTests
   {
      [Fact]
      public void ShouldImportHoleFileSamples()
      {
         var fs = new TestFileSystem
         {
            Files =
            {
               {@"x:\sample", new[] {"sample content"}},
               {@"x:\target", new[]
               {
                  "a",
                  "<!--<docs:sample src=\"sample\"/>-->",
                  "b"
               }}
            }
         };

         new Samples.Worker(fs).Execute(@"x:\target");

         fs.Files[@"x:\target"].ShouldBe(new[]
         {
            "a",
            "<!--<docs:sample src=\"sample\">-->",
            "```",
            "sample content",
            "```",
            "<!--</docs:sample>-->",
            "b"
         });
      }
   }
}