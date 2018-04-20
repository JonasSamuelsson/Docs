using Docs.Utils;
using Shouldly;
using Xunit;

namespace Docs.Tests.Utils
{
   public class RootDirectoryResolverTests
   {
      [Fact]
      public void ShouldResolveRootDirectory()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               {@"x:\project\.docsconfig.", new []{ "samples.dir:." } },
               {@"x:\project\src\foo.txt", new []{ "" } }
            }
         };

         new RootDirectoryResolver(fileSystem)
            .GetRootDirectory(@"x:\project\src\foo.txt")
            .ShouldBe(@"x:\project");
      }

      [Fact]
      public void ShouldSupportRootsOutsideOfTheCurrentDirectoryTree()
      {
         var fileSystem = new TestFileSystem
         {
            Files =
            {
               {@"x:\docs\.docsconfig.", new[] {"samples.dir:../samples"}},
               {@"x:\docs\foo.txt", new string[] { }}
            }
         };

         new RootDirectoryResolver(fileSystem)
            .GetRootDirectory(@"x:\docs\foo.txt")
            .ShouldBe(@"x:\samples");
      }
   }
}