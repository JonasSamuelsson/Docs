using Docs.Utils;
using Shouldly;
using Xunit;

namespace Docs.Tests.Utils
{
   public class SettingsReaderTests
   {
      [Fact]
      public void ShouldGetSamplesDirectory()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               {@"x:\project\.docs.", new []{ "samples.dir:." } },
               {@"x:\project\src\foo.txt", new []{ "" } }
            }
         };

         new SettingsReader(fileSystem)
            .GetSamplesDirectory(@"x:\project\src\foo.txt")
            .ShouldBe(@"x:\project");
      }

      [Fact]
      public void ShouldGetSampleLanguage()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               {@"x:\.docs.", new[] {"samples.languages.foo:bar"}},
               {@"x:\root.txt", new string[] { }}
            }
         };

         new SettingsReader(fileSystem)
            .TryGetSampleLanguage(@"x:\root.txt", ".FOO", out var language)
            .ShouldBe(true);

         language.ShouldBe("bar");
      }
   }
}