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
               {@"x:\project\.docsconfig.", new []{ "root:true" } },
               {@"x:\project\src\foo.txt", new []{ "" } }
            }
         };

         new RootDirectoryResolver(fileSystem)
            .GetRootDirectory(@"x:\project\src\foo.txt")
            .ShouldBe(@"x:\project");
      }
   }
}